namespace SecureNotes.API.Models.NoteDtos
{
    public class GetNoteDetailsRequestDto
    {
        public Guid NoteId { get; set; }
        public string? Password { get; set; }
    }
}