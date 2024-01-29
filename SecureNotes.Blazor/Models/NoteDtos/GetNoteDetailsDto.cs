namespace SecureNotes.API.Models.NoteDtos
{
    public class GetNoteDetailsDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool IsEncrypted { get; set; }
    }
}