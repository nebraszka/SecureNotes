using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Blazor.Models.UserDtos
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Proszę wprowadzić nazwę użytkownika.")]
        [MinLength(4, ErrorMessage = "Nazwa użytkownika musi być dłuższa niż 3 znaki.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Proszę podać adres e-mail.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu e-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Proszę podać hasło.")]
        [MinLength(8, ErrorMessage = "Hasło musi być dłuższe niż 8 znaków")]
        public string Password { get; set; }
    }
}