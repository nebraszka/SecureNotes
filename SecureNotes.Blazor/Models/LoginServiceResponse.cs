namespace SecureNotes.Blazor.Models
{
    public class LoginServiceResponse
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; } = null;
    }
}