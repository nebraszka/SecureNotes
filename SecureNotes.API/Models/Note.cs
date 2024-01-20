using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNotes.Shared.Models
{
    public class Note
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        [Required]
        [StringLength(30)]
        public string Title { get; set; }
        [Required]
        [StringLength(1000)]
        public string Content { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; } = false;
        public virtual User User { get; set; }
    }
}