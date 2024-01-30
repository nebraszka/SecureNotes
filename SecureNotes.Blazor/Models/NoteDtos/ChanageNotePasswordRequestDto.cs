namespace SecureNotes.API.Models.NoteDtos
{
    public class ChangeNotePasswordRequestDto
    {
        public Guid NoteId { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}