using Newtonsoft.Json;
using SecureNotes.API.Models.NoteDtos;
using SecureNotes.Blazor.Models;

namespace SecureNotes.Blazor.Services
{
    public class NoteService : INoteService
    {
        private readonly HttpClient _httpClient;
        private string urlPostfix = "Note";

        public NoteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<ServiceResponseWithoutData> ChangeNotePassword(Guid noteId, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> CreateNote(AddNoteDto newNote)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> DecryptNote(Guid noteId, string password)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> DeleteNote(Guid noteId, string? password)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> EncryptNote(Guid noteId, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<GetNoteDto>>> GetAllNotes()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{urlPostfix}/all");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponse<List<GetNoteDto>>>(responseBody)!;
                }
                else
                {
                    return new ServiceResponse<List<GetNoteDto>>
                    {
                        Success = false,
                        Message = "Błąd pobierania notatek"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponse<List<GetNoteDto>>
                {
                    Success = false,
                    Message = "Błąd pobierania notatek"
                };
            }
        }

        public async Task<ServiceResponse<List<GetNoteDto>>> GetAllPublicNotes()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{urlPostfix}/public");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponse<List<GetNoteDto>>>(responseBody)!;
                }
                else
                {
                    return new ServiceResponse<List<GetNoteDto>>
                    {
                        Success = false,
                        Message = "Błąd pobierania notatek"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponse<List<GetNoteDto>>
                {
                    Success = false,
                    Message = "Błąd pobierania notatek"
                };
            }
        }

        public async Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(Guid noteId, string? password)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{urlPostfix}/{noteId}{(password != null ? $"?password={password}" : "")}");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponse<GetNoteDetailsDto>>(responseBody)!;
                }
                else
                {
                    return new ServiceResponse<GetNoteDetailsDto>
                    {
                        Success = false,
                        Message = "Błąd pobierania notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponse<GetNoteDetailsDto>
                {
                    Success = false,
                    Message = "Błąd pobierania notatki"
                };
            }
        }

        public Task<ServiceResponseWithoutData> MakeNotePublic(Guid noteId, string? password)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> UpdateNote(Guid noteId, UpdateNoteDto updatedNote)
        {
            throw new NotImplementedException();
        }
    }
}