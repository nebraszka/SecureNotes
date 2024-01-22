using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace SecureNotes.API.Models.UserDtos
{
    public class LoginUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public string TOTPCode { get; set; }

        public bool IsValid()
        {
            // TODO 
            return !string.IsNullOrWhiteSpace(Email) || 
                   !string.IsNullOrWhiteSpace(Username) && 
                   !string.IsNullOrWhiteSpace(Password);
        }
    }
}