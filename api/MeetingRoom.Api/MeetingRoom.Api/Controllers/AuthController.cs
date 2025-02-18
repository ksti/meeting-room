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
        public async Task<ApiResponse<AuthResult>> Register([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ApiResponse<AuthResult>.BadRequest(ModelState);
            }
            // create user
            var currentUser = await currentUserService.GetCurrentUserAsync();
            var result = await authService.CreateAsync(request, currentUser!.Id);

            return string.IsNullOrEmpty(result.AccessToken) ? ApiResponse<AuthResult>.Ok(result) : ApiResponse<AuthResult>.BadRequest();
        }

        [HttpPost("login")]
        public async Task<ApiResponse<AuthResult>> Login(string email, string password)
        {
            var result = await authService.LoginAsync(email, password);

            return ApiResponse<AuthResult>.Ok(result);
        }

    }
}
