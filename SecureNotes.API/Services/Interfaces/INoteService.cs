using SecureNotes.API.Models;
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.Shared.Models;

namespace SecureNotes.API.Services.Interfaces
{
    public interface INoteService
    {
        Task<ServiceResponse<List<GetNoteDto>>> GetAllPublicNotes();
        Task<ServiceResponse<List<GetNoteDto>>> GetAllNotes(Guid userId);
        Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(Guid userId, Guid noteId, string? password);
        Task<ServiceResponseWithoutData> CreateNote(Guid userId, AddNoteDto newNote);
        Task<ServiceResponseWithoutData> UpdateNote(Guid userId, Guid noteId, UpdateNoteDto updatedNote);
        Task<ServiceResponseWithoutData> DeleteNote(Guid userId, Guid noteId, string? password);
        Task<ServiceResponseWithoutData> EncryptNote(Guid userId, Guid noteId, string password);
        Task<ServiceResponseWithoutData> DecryptNote(Guid userId, Guid noteId, string password);
        Task<ServiceResponseWithoutData> MakeNotePublic(Guid userId, Guid noteId, string? password);

        Task<ServiceResponseWithoutData> ChangeNotePassword(Guid userId, Guid noteId, string oldPassword, string newPassword);
    }
}