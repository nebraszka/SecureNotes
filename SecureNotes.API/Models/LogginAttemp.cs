using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNotes.Shared.Models
{
    public class LoginAttempt
    {
        [Key]
        public Guid LoginAttemptId { get; set; }
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public bool Success { get; set; }
        public string IpAddress { get; set; }
        public DateTime Time { get; set; }
    }
}