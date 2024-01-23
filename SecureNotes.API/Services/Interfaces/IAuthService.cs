using SecureNotes.API.Models;
using SecureNotes.API.Models.UserDtos;

namespace SecureNotes.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Login(LoginUserDto loginUserDto, string ipAddress);
        Task<ServiceResponse<RegisteredUserDto>> Register(RegisterUserDto registerUserDto);
    }
}