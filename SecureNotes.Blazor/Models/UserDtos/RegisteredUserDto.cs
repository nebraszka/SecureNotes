using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNotes.Blazor.Models.UserDtos
{
    public class RegisteredUserDto
    {
        public string TOTPSecret { get; set; }
    }
}