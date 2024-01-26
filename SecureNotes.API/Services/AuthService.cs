using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OtpNet;

using SecureNotes.API.Data;
using SecureNotes.API.Encryption;
using SecureNotes.API.Models;
using SecureNotes.API.Models.UserDtos;
using SecureNotes.API.Services.Interfaces;
using SecureNotes.Shared.Models;

namespace SecureNotes.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        private readonly JwtSettings _jwtSettings;

        public AuthService(DataContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        private string loginFailedMessage = "Logowanie nie powiodło się";
        private string loginSuccessMessage = "Logowanie powiodło się";
        private string registerFailedMessage = "Rejestracja nie powiodła się";
        private string registerSuccessMessage = "Rejestracja powiodła się";
        private string userExistsMessage = "Użytkownik o podanej nazwie lub adresie email już istnieje";

        public async Task<ServiceResponse<string>> Login(LoginUserDto loginUserDto, string ipAddress)
        {
            if (!await UserExists(loginUserDto.Username, loginUserDto.Email))
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = loginFailedMessage
                };
            }

            var currentUser = await GetUser(loginUserDto);
            if(currentUser == null)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = loginFailedMessage
                };
            }

            // Delay login attempt if there were failed login attempts in last 15 minutes
            await SetLoginDelay(currentUser);

            // Check if account is locked
            if (currentUser!.IsAccountLocked && currentUser.AccountLockoutEnd > DateTime.Now)
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Konto jest zablokowane"
                };
            }

            // Add new login attempt
            LoginAttempt loginAttempt = new LoginAttempt
            {
                UserId = currentUser!.UserId,
                IpAddress = ipAddress,
                Time = DateTime.Now
            };

            // Verify password hash
            using (var hmac = new HMACSHA512(currentUser.PasswordSalt))
            {
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginUserDto.Password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != currentUser.PasswordHash[i])
                    {
                        loginAttempt.Success = false;
                        _context.LoginAttempts!.Add(loginAttempt);
                        await _context.SaveChangesAsync();

                        string loginAttemptMsg = await CheckLoginAttempts(currentUser);

                        return new ServiceResponse<string>
                        {
                            Data = null,
                            Success = false,
                            Message = string.IsNullOrEmpty(loginAttemptMsg) ? loginFailedMessage : loginAttemptMsg
                        };
                    }
                }
            }

            // Verify TOTP
            byte[] keyToDecrypt = AesEncryption.CreateAesKeyFromPassword(loginUserDto.Password, currentUser.PasswordSalt);
            string decryptedTOTP = AesEncryption.Decrypt(Convert.ToBase64String(currentUser.TOTPSecret), Convert.ToBase64String(keyToDecrypt), currentUser.Iv);
            byte[] totpToConvert = Convert.FromBase64String(decryptedTOTP);

            var totp = new Totp(totpToConvert);
            if (!totp.VerifyTotp(loginUserDto.TOTPCode, out long timeStepMatched, new VerificationWindow(2, 2)))
            {
                loginAttempt.Success = false;
                _context.LoginAttempts!.Add(loginAttempt);
                await _context.SaveChangesAsync();

                string loginAttemptMsg = await CheckLoginAttempts(currentUser);

                return new ServiceResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = string.IsNullOrEmpty(loginAttemptMsg) ? loginFailedMessage : loginAttemptMsg
                };
            }

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Email, currentUser.Email),
                new Claim(ClaimTypes.NameIdentifier, currentUser.UserId.ToString()),
                new Claim(ClaimTypes.Name, currentUser.Username),
            }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var stringToken = tokenHandler.WriteToken(token);

            loginAttempt.Success = true;
            _context!.LoginAttempts!.Add(loginAttempt);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string>
            {
                Data = stringToken,
                Success = true,
                Message = loginSuccessMessage
            };
        }

        // TODO Consider computing entropy of password
        public async Task<ServiceResponse<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            // Check if user exists
            if (await _context!.Users!.AnyAsync(u => u.Username == registerUserDto.Username || u.Email == registerUserDto.Email))
            {
                return new ServiceResponse<RegisteredUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = userExistsMessage
                };
            }

            // Generate IV
            string iv;
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();
                iv = Convert.ToBase64String(aes.IV);
            }

            // Generate new user
            User newUser;
            using (var hmac = new HMACSHA512())
            {
                newUser = new User
                {
                    Username = registerUserDto.Username,
                    Email = registerUserDto.Email,
                    PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerUserDto.Password)),
                    // Using salt to prevent rainbow table attacks
                    PasswordSalt = hmac.Key,
                    // Notes = new List<Note>(),
                    LoginAttempts = new List<LoginAttempt>(),
                    TOTPSecret = KeyGeneration.GenerateRandomKey(20),
                    Iv = iv
                };
            }

            // Return TOTP secret to user
            RegisteredUserDto registeredUserDto = new RegisteredUserDto
            {
                TOTPSecret = Base32Encoding.ToString(newUser.TOTPSecret)
            };

            // Encrypt TOTP secret before saving to database
            byte[] key = AesEncryption.CreateAesKeyFromPassword(registerUserDto.Password, newUser.PasswordSalt);
            string totpSecretBase64 = Convert.ToBase64String(newUser.TOTPSecret);
            newUser.TOTPSecret = Convert.FromBase64String(AesEncryption.Encrypt(totpSecretBase64, Convert.ToBase64String(key), newUser.Iv));

            try
            {
                _context.Users!.Add(newUser);
                await _context.SaveChangesAsync();
                return new ServiceResponse<RegisteredUserDto>
                {
                    Data = registeredUserDto,
                    Success = true,
                    Message = registerSuccessMessage
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<RegisteredUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = registerFailedMessage
                };
            }
        }

        private async Task<User?> GetUser(LoginUserDto loginUserDto)
        {
            return !string.IsNullOrWhiteSpace(loginUserDto.Username)
                ? await _context!.Users!.FirstOrDefaultAsync(u => u.Username == loginUserDto.Username)
                : await _context!.Users!.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email);
        }

        private async Task SetLoginDelay(User user)
        {
            // Delay login attempt if there were failed login attempts in last 15 minutes
            var failedLoginAttemptsCount = await _context.LoginAttempts!.CountAsync(a => a.UserId == user.UserId && a.Success == false && a.Time > DateTime.Now.AddMinutes(-15));

            if (failedLoginAttemptsCount > 0)
            {
                int delay = Math.Min(30000, 1000 * (int)Math.Pow(2, failedLoginAttemptsCount - 1));
                await Task.Delay(delay);
            }
        }

        private async Task AddLoginAttempt(User user, string ipAddress, bool isSuccess)
        {
            LoginAttempt loginAttempt = new LoginAttempt
            {
                UserId = user.UserId,
                IpAddress = ipAddress,
                Time = DateTime.Now,
                Success = isSuccess
            };

            _context!.LoginAttempts!.Add(loginAttempt);
            await _context.SaveChangesAsync();
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(storedHash);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings!.Secret!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<bool> UserExists(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return await _context!.Users!.AnyAsync(u => u.Email == email) || await _context!.Users!.AnyAsync(u => u.Username == username);
        }

        private async Task<string> CheckLoginAttempts(User user)
        {
            // Check failed login attempts in last 15 minutes
            var failedLoginAttemptsCount = await _context.LoginAttempts!.CountAsync(a => a.UserId == user.UserId && a.Success == false && a.Time > DateTime.Now.AddMinutes(-15));

            if (failedLoginAttemptsCount >= 5)
            {
                user.IsAccountLocked = true;
                user.AccountLockoutEnd = DateTime.Now.AddMinutes(15);
                await _context.SaveChangesAsync();
                return loginFailedMessage + ". " + "Konto zostało zablokowane na 15 minut";
            }

            return string.Empty;
        }
    }
}