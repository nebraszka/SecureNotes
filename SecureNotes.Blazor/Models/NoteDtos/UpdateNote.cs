using System.ComponentModel.DataAnnotations;

namespace SecureNotes.API.Models.NoteDtos
{
    public class UpdateNoteDto
    {
        public Guid NoteId { get; set; }
        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        [StringLength(50, ErrorMessage = "Tytuł może mieć maksymalnie 50 znaków.")]
        [RegularExpression(@"^[a-zA-Z0-9\sąćęłńóśźżĄĆĘŁŃÓŚŹŻ]*$", ErrorMessage = "Tytuł może zawierać tylko litery, cyfry i spacje.")]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
    }
}