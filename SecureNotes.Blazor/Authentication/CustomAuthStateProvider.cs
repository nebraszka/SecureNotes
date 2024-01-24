using System.Security.Claims;
using System.Text.Json;
using SecureNotes.Blazor.Services.Interfaces;

namespace SecureNotes.Blazor.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorageService;

        public CustomAuthStateProvider(ILocalStorageService localStorageService)
        {
            this.localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorageService.GetItemAsync<string>("token");

            var identinty = new ClaimsIdentity();

            if(!string.IsNullOrEmpty(token))
            {
                identinty = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            }

            var user = new ClaimsPrincipal(identinty);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            return state;
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }

}