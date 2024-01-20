using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNotes.Shared.Models
{
public class User
{
    [Key]
    public Guid UserId { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(30)]
    public string Username { get; set; }
    [Required]
    public byte[] PasswordHash { get; set; }
    [Required]
    public byte[] PasswordSalt { get; set; }
    [Required]
    public byte[] TOTPSecret { get; set; }
    [Required]
    public string Iv { get; set; }

    public virtual ICollection<Note> Notes { get; set; }

    public ICollection<LoginAttempt> LoginAttempts { get; set; }
}
}