namespace SecureNotes.API.Models.NoteDtos
{
    public class MakeNotePublicRequestDto
    {
        public Guid NoteId { get; set; }
        public string? Password { get; set; }
    }
}