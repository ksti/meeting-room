using MeetingRoom.Api.Models;
using MeetingRoom.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MeetingRoom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController(ILogger<AuthController> logger, IAuthService authService, ICurrentUserService currentUserService) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ApiResponse<AuthResult>> Register([FromBody] UserCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse<AuthResult>.BadRequest(ModelState);
            }
            var result = await authService.RegisterAsync(request, request.DeviceInfo);

            return ApiResponse<AuthResult>.Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ApiResponse<AuthResult>> Login(string email, string password, DeviceInfo deviceInfo)
        {
            var result = await authService.LoginAsync(email, password, deviceInfo);

            return ApiResponse<AuthResult>.Ok(result);
        }

        [HttpPost("logout")]
        public async Task<ApiResponse<bool>> Logout()
        {
            var currentUser = await currentUserService.GetCurrentUserAsync();
            var result = await authService.LogoutAsync(currentUser!.Email);

            return ApiResponse<bool>.Ok(result);
        }

    }
}
