namespace SecureNotes.API.Models
{
    public class ServiceResponseWithoutData
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; } = null;
    }
}