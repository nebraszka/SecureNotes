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

        public async Task<ServiceResponse<string>> Login(LoginUserDto loginUserDto, string ipAddress)
        {
            // Check if user exists
            if (!await UserExists(loginUserDto.Username, loginUserDto.Email))
            {
                return new ServiceResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Podany użytkownik nie istnieje"
                };
            }

            // Get user
            User? currentUser = null;

            if (!string.IsNullOrWhiteSpace(loginUserDto.Username))
            {
                currentUser = await _context!.Users!.FirstOrDefaultAsync(u => u.Username == loginUserDto.Username);
            }
            else if (!string.IsNullOrWhiteSpace(loginUserDto.Email))
            {
                currentUser = await _context!.Users!.FirstOrDefaultAsync(u => u.Email == loginUserDto.Email);
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
                        return new ServiceResponse<string>
                        {
                            Data = null,
                            Success = false,
                            Message = "Logowanie nie powiodło się"
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
                return new ServiceResponse<string>
                {
                    Data = null,
                    Success = false,
                    Message = "Logowanie nie powiodło się"
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
                Success = true,
                Message = "Logowanie powiodło się",
                Data = stringToken
            };
        }

        // TODO Consider computing entropy of password
        public async Task<ServiceResponse<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            // Check if user exists
            if (await UserExists(registerUserDto.Username, registerUserDto.Email))
            {
                return new ServiceResponse<RegisteredUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Taki użytkownik już istnieje"
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
                    Notes = new List<Note>(),
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

            // Encrypt TOTP secret
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
                    Message = "Użytkownik został zarejestrowany"
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<RegisteredUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Wystąpił błąd podczas rejestracji użytkownika"
                };
            }
        }

        // TODO
        private async Task<bool> UserExists(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return await _context!.Users!.AnyAsync(u => u.Email == email);
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                return await _context!.Users!.AnyAsync(u => u.Username == username);
            }

            return await _context!.Users!.AnyAsync(u => u.Username == username && u.Email == email);
        }
    }
}