using System.ComponentModel.DataAnnotations;

namespace SecureNotes.Shared.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; } = new byte[0];
        [Required]
        public byte[] PasswordSalt { get; set; } = new byte[0];
        [Required]
        public byte[] TOTPSecret { get; set; } = new byte[0];
        [Required]
        public string Iv { get; set; } = string.Empty;

        public bool IsAccountLocked { get; set; }
        public DateTime? AccountLockoutEnd { get; set; }

        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();

        public ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
    }
}