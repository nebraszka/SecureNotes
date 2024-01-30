namespace SecureNotes.API.Models.NoteDtos
{
    public class EncryptNoteRequestDto
    {
        public Guid NoteId { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}