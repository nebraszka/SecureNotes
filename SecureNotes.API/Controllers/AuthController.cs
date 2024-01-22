using Microsoft.AspNetCore.Mvc;
using SecureNotes.API.Data;
using SecureNotes.API.Models;
using SecureNotes.API.Models.UserDtos;
using SecureNotes.API.Services;
using SecureNotes.API.Services.Interfaces;

namespace SecureNotes.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(DataContext context)
        {
            _authService = new AuthService(context);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<LoggedInUserDto>>> Login(LoginUserDto loginUserDto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();

            var response = await _authService.Login(loginUserDto, ipAddress!);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<RegisteredUserDto>>> Register(RegisterUserDto registerUserDto)
        {
            var response = await _authService.Register(registerUserDto);

            return Ok(response);
        }
    }
}