namespace SecureNotes.API.Models.NoteDtos
{
    public class AddNoteDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = false;
        public DateTime CreationDate { get; set; }
        public bool IsEncrypted { get; set; } = false;
        public string? Password { get; set; } = string.Empty;
    }
}