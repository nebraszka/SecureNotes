
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.Blazor.Models;

namespace SecureNotes.Blazor.Services
{
    public interface INoteService
    {
        Task<ServiceResponse<List<GetNoteDto>>> GetAllPublicNotes();
        Task<ServiceResponse<List<GetNoteDto>>> GetAllNotes();
        Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(Guid noteId, string? password);
        Task<ServiceResponseWithoutData> CreateNote(AddNoteDto newNote);
        Task<ServiceResponseWithoutData> UpdateNote(Guid noteId, UpdateNoteDto updatedNote);
        Task<ServiceResponseWithoutData> DeleteNote(Guid noteId, string? password);
        Task<ServiceResponseWithoutData> EncryptNote(Guid noteId, string password);
        Task<ServiceResponseWithoutData> DecryptNote(Guid noteId, string password);
        Task<ServiceResponseWithoutData> MakeNotePublic(Guid noteId, string? password);

        Task<ServiceResponseWithoutData> ChangeNotePassword(Guid noteId, string oldPassword, string newPassword);
    }
}