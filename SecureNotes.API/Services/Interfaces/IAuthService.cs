using SecureNotes.API.Models;
using SecureNotes.API.Models.UserDtos;

namespace SecureNotes.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<LoggedInUserDto>> Login(LoginUserDto loginUserDto, string ipAddress);
        Task<ServiceResponse<RegisteredUserDto>> Register(RegisterUserDto registerUserDto);
    }
}