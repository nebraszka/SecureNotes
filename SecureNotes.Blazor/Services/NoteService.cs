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

        public async Task<ServiceResponseWithoutData> ChangeNotePassword(ChangeNotePasswordRequestDto changeNotePasswordRequest)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(changeNotePasswordRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/change-password", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd zmiany hasła notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd zmiany hasła notatki"
                };
            }
        }

        public async Task<ServiceResponseWithoutData> CreateNote(AddNoteDto newNote)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(newNote), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/create", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd tworzenia notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd tworzenia notatki"
                };
            }
        }

        public async Task<ServiceResponseWithoutData> DecryptNote(DecryptNoteRequestDto decryptNoteRequest)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(decryptNoteRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/decrypt", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd odszyfrowywania notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd odszyfrowywania notatki"
                };
            }
        }


        public async Task<ServiceResponseWithoutData> DeleteNote(DeleteNoteRequestDto deleteNoteRequest)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(deleteNoteRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/delete", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd usuwania notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd usuwania notatki"
                };
            }
        }

        public async Task<ServiceResponseWithoutData> EncryptNote(EncryptNoteRequestDto encryptNoteRequest)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(encryptNoteRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/encrypt", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd szyfrowania notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd szyfrowania notatki"
                };
            }
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

        public async Task<ServiceResponseWithoutData> MakeNotePrivate(MakeNotePrivateRequestDto makeNotePrivateRequest)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(makeNotePrivateRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/make-private", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd prywatyzacji notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd prywatyzacji notatki"
                };
            }
        }

        public async Task<ServiceResponseWithoutData> MakeNotePublic(MakeNotePublicRequestDto makeNotePublicRequest)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(makeNotePublicRequest), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/make-public", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd udostępniania notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd udostępniania notatki"
                };
            }
        }

        public async Task<ServiceResponseWithoutData> UpdateNote(UpdateNoteDto updatedNote)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(updatedNote), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{urlPostfix}", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponseWithoutData>(responseBody)!;
                }
                else
                {
                    return new ServiceResponseWithoutData
                    {
                        Success = false,
                        Message = "Błąd aktualizacji notatki"
                    };
                }
            }
            catch (Exception)
            {
                return new ServiceResponseWithoutData
                {
                    Success = false,
                    Message = "Błąd aktualizacji notatki"
                };
            }
        }
    }
}