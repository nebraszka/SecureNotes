using SecureNotes.Blazor.Models;
using SecureNotes.Blazor.Models.UserDtos;
using SecureNotes.Blazor.Services.Interfaces;

using Newtonsoft.Json;
using System.Text;

namespace SecureNotes.Blazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private string urlPostfix = "Auth";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<string>> Login(LoginUserDto loginUserDto)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(loginUserDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/login", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponse<string>>(responseBody)!;
                }
                else
                {
                    return new ServiceResponse<string>
                    {
                        Success = false,
                        Message = "Błąd logowania"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    Message = "Błąd logowania"
                };
            }
        }

        public async Task<ServiceResponse<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(registerUserDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/register", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponse<RegisteredUserDto>>(responseBody)!;
                }
                else
                {
                    return new ServiceResponse<RegisteredUserDto>
                    {
                        Success = false,
                        Message = "Wystąpił błąd podczas rejestracji: " + response.StatusCode + " " + response.ReasonPhrase
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<RegisteredUserDto>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas rejestracji: " + ex.Message
                };
            }
        }
    }
}
