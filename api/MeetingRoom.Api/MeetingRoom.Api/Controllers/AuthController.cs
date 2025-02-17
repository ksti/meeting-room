using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeetingRoom.Api.Models;
using MeetingRoom.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MeetingRoom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(ILogger<AuthController> logger, IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel userModel)
        {
            // create user
            var user = new IdentityUser { UserName = userModel.Email, Email = userModel.Email };
            var result = await authService.CreateAsync(user, "P@ssw0rd");
            if (string.IsNullOrEmpty(result.AccessToken))
            {
                return BadRequest();
            }

            // generate jwt token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, student.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yoursecretkey")), SecurityAlgorithms.HmacSha256)
            );

            // return jwt token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

    }
}
