using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
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
        // TODO Add JWT Token Service

        public AuthService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<LoggedInUserDto>> Login(LoginUserDto loginUserDto, string ipAddress)
        {
            // Check if user exists
            if (!await UserExists(loginUserDto.Username, loginUserDto.Email))
            {
                return new ServiceResponse<LoggedInUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "User does not exist"
                };
            }

            // Get user
            User? currentUser = await _context!.Users!.FirstOrDefaultAsync(u => u.Username == loginUserDto.Username);

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
                        return new ServiceResponse<LoggedInUserDto>
                        {
                            Data = null,
                            Success = false,
                            Message = "Login failed - wrong hash"
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
                return new ServiceResponse<LoggedInUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Login failed - wrong TOTP"
                };
            }
            
            // Return JWT token - for now hardcoded - TODO

            loginAttempt.Success = true;
            _context!.LoginAttempts!.Add(loginAttempt);
            await _context.SaveChangesAsync();
            return new ServiceResponse<LoggedInUserDto>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoggedInUserDto { Jwt = "JWT Token" }
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
                    Message = "User already exists"
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
                    Message = "User registered successfully"
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse<RegisteredUserDto>
                {
                    Data = null,
                    Success = false,
                    Message = "Registration failed"
                };
            }
        }

        private async Task<bool> UserExists(string username, string email)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            if (await _context!.Users!.AnyAsync(u => u.Username == username && u.Email == email))
            {
                return true;
            }

            return false;
        }
    }
}