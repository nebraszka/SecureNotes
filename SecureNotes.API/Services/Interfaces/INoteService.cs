using SecureNotes.API.Models;
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.Shared.Models;

namespace SecureNotes.API.Services.Interfaces
{
    public interface INoteService
    {
        Task<ServiceResponse<List<GetNoteDto>>> GetAllPublicNotes();
        Task<ServiceResponse<List<GetNoteDto>>> GetAllNotes(Guid userId);
        Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(Guid userId, GetNoteDetailsRequestDto getNoteDetailsRequest);
        Task<ServiceResponseWithoutData> CreateNote(Guid userId, AddNoteDto newNote);
        Task<ServiceResponseWithoutData> UpdateNote(Guid userId, UpdateNoteDto updatedNote);
        Task<ServiceResponseWithoutData> DeleteNote(Guid userId, DeleteNoteRequestDto deleteNoteRequest);
        Task<ServiceResponseWithoutData> EncryptNote(Guid userId, EncryptNoteRequestDto encryptNoteRequest);
        Task<ServiceResponseWithoutData> DecryptNote(Guid userId, DecryptNoteRequestDto decryptNoteRequest);
        Task<ServiceResponseWithoutData> MakeNotePublic(Guid userId, MakeNotePublicRequestDto makeNotePublicRequest);
        Task<ServiceResponseWithoutData> ChangeNotePassword(Guid userId, ChangeNotePasswordRequestDto changeNotePasswordRequest);
    }
}