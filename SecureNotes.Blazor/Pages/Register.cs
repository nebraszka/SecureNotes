using Microsoft.AspNetCore.Components;
using SecureNotes.Blazor.Models.UserDtos;
using SecureNotes.Blazor.Services.Interfaces;

namespace SecureNotes.Blazor.Pages
{
    public partial class Register : ComponentBase
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IAuthService AuthService { get; set; }

        private RegisterUserDto registerUserDto = new RegisterUserDto();
        private string registrationResult = string.Empty;
        private bool passwordVisible = false;
        private string passwordFieldType = "password";

        private void TogglePasswordVisibility()
        {
            passwordVisible = !passwordVisible;
            passwordFieldType = passwordVisible ? "text" : "password";
        }

        private async Task HandleValidSubmit()
        {
            var result = await AuthService.Register(registerUserDto);
            if (!result.Success)
            {
                registrationResult = result.Message!;
                return;
            }

            registrationResult = $"Rejestracja udana. Twój sekret TOTP to: {result.Data!.TOTPSecret}";
        }
    }
}