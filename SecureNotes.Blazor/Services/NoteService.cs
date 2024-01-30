using System.Text;
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

        public Task<ServiceResponseWithoutData> ChangeNotePassword(ChangeNotePasswordRequestDto changeNotePasswordRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> CreateNote(AddNoteDto newNote)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> DecryptNote(DecryptNoteRequestDto decryptNoteRequest)
        {
            throw new NotImplementedException();
        }


        public Task<ServiceResponseWithoutData> DeleteNote(DeleteNoteRequestDto deleteNoteRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> EncryptNote(EncryptNoteRequestDto encryptNoteRequest)
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

        public async Task<ServiceResponse<GetNoteDetailsDto>> GetNoteDetails(GetNoteDetailsRequestDto getNoteDetailsRequest)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(getNoteDetailsRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/details", itemJson);
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

        public Task<ServiceResponseWithoutData> MakeNotePublic(MakeNotePublicRequestDto makeNotePublicRequest)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponseWithoutData> UpdateNote(UpdateNoteDto updatedNote)
        {
            throw new NotImplementedException();
        }
    }
}