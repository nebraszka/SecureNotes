using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SecureNotes.Blazor.Models.UserDtos;
using SecureNotes.Blazor.Services.Interfaces;

namespace SelfAID.WebClient.Pages;

public partial class Login : ComponentBase
{
    [Inject]
    private IAuthService authService { get; set; }

    // [Inject]
    // private AuthenticationStateProvider authenticationStateProvider { get; set; }

    [Inject]
    private NavigationManager navigationManager { get; set; }

    [Inject]
    private HttpClient Http { get; set; }

    protected string Message = string.Empty;
    public LoginUserDto user = new LoginUserDto();

    protected async Task HandleLogin()
    {
        // TODO make it better
        if(user.Username.Contains("@"))
        {
            user.Email = user.Username;
            user.Username = string.Empty;
        }

        var response = await authService.Login(user);
        if (response != null && response.Success)
        {
            // tokenService.SetToken(response.Data);

            // var customAuthProvider = authenticationStateProvider as CustomAuthStateProvider;
            // customAuthProvider?.NotifyUserAuthentication(response.Data);
            
            navigationManager.NavigateTo("/");
        }
        else
        {
            Message = response?.Message ?? "Błąd logowania";
        
        }
    }
}