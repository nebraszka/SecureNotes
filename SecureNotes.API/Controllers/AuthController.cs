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

        public AuthController(DataContext context, JwtSettings jwtSettings)
        {
            _authService = new AuthService(context, jwtSettings);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginServiceResponse>> Login(LoginUserDto loginUserDto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();

            var response = await _authService.Login(loginUserDto, ipAddress!);

            var encryptorToken = response.Data;
            if(encryptorToken == null)
            {
                return Unauthorized();
            }

            // Pass jwt inside HttpOnly cookie
            HttpContext.Response.Cookies.Append("jwt", encryptorToken!,
                new CookieOptions
                {
                    HttpOnly = true, // So JavaScript can't access the cookie
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7),
                    IsEssential = true
                });

            return Ok();
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<RegisteredUserDto>>> Register(RegisterUserDto registerUserDto)
        {
            var response = await _authService.Register(registerUserDto);

            return Ok(response);
        }
    }
}