using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SecureNotes.Blazor.Models.UserDtos;
using SecureNotes.Blazor.Services.Interfaces;

namespace SecureNotes.Blazor.Pages
{
    public partial class Register : ComponentBase
    {
        [Inject]
        private IAuthService AuthService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private RegisterUserDto registerUserDto = new RegisterUserDto();
        private string registrationResult = string.Empty;
        private bool isQRCodeVisible = false;
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

            isQRCodeVisible = true;
            StateHasChanged(); // Zmusza do ponownego renderowania komponentu

            // Użyj Task.Delay, aby dać czas na wyrenderowanie elementu
            await Task.Delay(100);
            await JSRuntime.InvokeVoidAsync("generateQRCode", result.Data!.TOTPSecret);
        }

        private void UpdatePassword(ChangeEventArgs e)
        {
            registerUserDto.Password = e.Value.ToString();
        }
    }
}