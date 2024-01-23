using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNotes.Blazor.Models.UserDtos
{
    public class LoggedInUserDto
    {
        public string Jwt { get; set; }
    }
}