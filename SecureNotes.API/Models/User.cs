using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Shared.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public byte[] TOTPSecret { get; set; }
        [Required]
        public string Iv { get; set; }

        public bool IsAccountLocked { get; set; }
        public DateTime? AccountLockoutEnd { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public ICollection<LoginAttempt> LoginAttempts { get; set; }
    }
}