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

        public async Task<ServiceResponse<LoggedInUserDto>> Login(LoginUserDto loginUserDto)
        {
            try
            {
                var itemJson = new StringContent(JsonConvert.SerializeObject(loginUserDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{urlPostfix}/login", itemJson);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ServiceResponse<LoggedInUserDto>>(responseBody)!;
                }
                else
                {
                    return new ServiceResponse<LoggedInUserDto>
                    {
                        Success = false,
                        Message = "Wystąpił błąd podczas logowania: " + response.StatusCode + " " + response.ReasonPhrase
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoggedInUserDto>
                {
                    Success = false,
                    Message = "Wystąpił błąd podczas logowania: " + ex.Message
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
