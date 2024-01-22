using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNotes.API.Models.UserDtos
{
    public class RegisteredUserDto
    {
        public string TOTPSecret { get; set; }
    }
}