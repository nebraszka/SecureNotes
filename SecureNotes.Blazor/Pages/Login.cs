using Microsoft.AspNetCore.Components;
using SecureNotes.Blazor.Authentication;
using SecureNotes.Blazor.Models.UserDtos;
using SecureNotes.Blazor.Services.Interfaces;

namespace SelfAID.WebClient.Pages;

public partial class Login : ComponentBase
{
    [Inject]
    private IAuthService? authService { get; set; }

    [Inject]
    private AuthenticationStateProvider? authenticationStateProvider { get; set; }

    [Inject]
    private NavigationManager? navigationManager { get; set; }

    [Inject]
    private HttpClient? Http { get; set; }

    [Inject]
    private ILocalStorageService? localStorageService { get; set; }

    protected string Message = string.Empty;
    public LoginUserDto user = new LoginUserDto();
    
    private bool passwordVisible = false;
    private string passwordFieldType = "password";

    private void TogglePasswordVisibility()
    {
        passwordVisible = !passwordVisible;
        passwordFieldType = passwordVisible ? "text" : "password";
    }

    protected async Task HandleLogin()
    {
        // TODO make it better
        if (user.Username.Contains("@"))
        {
            user.Email = user.Username;
            user.Username = string.Empty;
        }

        var response = await authService!.Login(user);
        if (response.Success)
        {
            var token = response.Data;
            await ((CustomAuthStateProvider)authenticationStateProvider!).MarkUserAsAuthenticated(token!);
            navigationManager!.NavigateTo("/");
        }
        else
        {
            Message = response?.Message ?? "Błąd logowania";

        }
    }
}