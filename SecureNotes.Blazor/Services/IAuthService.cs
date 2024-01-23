using SecureNotes.Blazor.Models;
using SecureNotes.Blazor.Models.UserDtos;

namespace SecureNotes.Blazor.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<LoggedInUserDto>> Login(LoginUserDto loginUserDto);
        Task<ServiceResponse<RegisteredUserDto>> Register(RegisterUserDto registerUserDto);
    }
}