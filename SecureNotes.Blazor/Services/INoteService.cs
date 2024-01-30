
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.Blazor.Models;

namespace SecureNotes.Blazor.Services
{
    public interface INoteService
    {
        Task<ServiceResponse<List<GetNoteDto>>> GetAllPublicNotes();
        Task<ServiceResponse<List<GetNoteDto>>> GetAllNotes();
        Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(GetNoteDetailsRequestDto getNoteDetailsRequest);
        Task<ServiceResponseWithoutData> CreateNote(AddNoteDto newNote);
        Task<ServiceResponseWithoutData> UpdateNote(UpdateNoteDto updatedNote);
        Task<ServiceResponseWithoutData> DeleteNote(DeleteNoteRequestDto deleteNoteRequest);
        Task<ServiceResponseWithoutData> EncryptNote(EncryptNoteRequestDto encryptNoteRequest);
        Task<ServiceResponseWithoutData> DecryptNote(DecryptNoteRequestDto decryptNoteRequest);
        Task<ServiceResponseWithoutData> MakeNotePublic(MakeNotePublicRequestDto makeNotePublicRequest);
        Task<ServiceResponseWithoutData> ChangeNotePassword(ChangeNotePasswordRequestDto changeNotePasswordRequest);
    }
}