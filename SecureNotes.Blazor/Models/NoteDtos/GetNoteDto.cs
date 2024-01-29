namespace SecureNotes.API.Models.NoteDtos
{
    public class GetNoteDto
    {
        public Guid NoteId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool IsEncrypted { get; set; }
    }
}