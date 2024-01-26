namespace SecureNotes.API.Models.NoteDtos
{
    public class UpdateNoteDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; }
        public bool IsEncrypted { get; set; }
        public string? Password { get; set; } = string.Empty;
    }
}