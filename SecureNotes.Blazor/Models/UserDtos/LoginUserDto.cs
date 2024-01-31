using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Blazor.Models.UserDtos
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "Proszę wprowadzić nazwę użytkownika.")]
        [MinLength(4, ErrorMessage = "Nazwa użytkownika musi być dłuższa niż 3 znaki.")]
        [MaxLength(20, ErrorMessage = "Nazwa użytkownika musi być krótsza niż 21 znaków.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Nazwa użytkownika może zawierać tylko litery i cyfry.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Proszę podać adres e-mail.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Proszę podać kod TOTP.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Kod TOTP musi składać się z 6 cyfr.")]
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