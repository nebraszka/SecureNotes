namespace SecureNotes.API.Models.NoteDtos
{
    public class DecryptNoteRequestDto
    {
        public Guid NoteId { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}