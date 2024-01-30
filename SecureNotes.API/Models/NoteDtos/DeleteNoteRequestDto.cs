namespace SecureNotes.API.Models.NoteDtos
{
    public class DeleteNoteRequestDto
    {
        public Guid NoteId { get; set; }
        public string? Password { get; set; }
    }
}