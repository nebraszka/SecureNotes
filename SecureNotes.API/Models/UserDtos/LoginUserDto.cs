using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace SecureNotes.API.Models.UserDtos
{
    public class LoginUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string TOTPCode { get; set; } = string.Empty;

        public bool IsValid()
        {
            // TODO 
            return !string.IsNullOrWhiteSpace(Email) || 
                   !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Password);
        }
    }
}