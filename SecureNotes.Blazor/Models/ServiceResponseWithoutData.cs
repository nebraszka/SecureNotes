namespace SecureNotes.Blazor.Models
{
    public class ServiceResponseWithoutData
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; } = null;
    }
}