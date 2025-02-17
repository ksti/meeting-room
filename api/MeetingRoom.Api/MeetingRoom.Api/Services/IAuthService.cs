using MeetingRoom.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace MeetingRoom.Api.Services
{
    public interface IAuthService
    {
        Task<bool> VerifyPasswordAsync(string email, string password);
        Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<bool> LogoutAsync(string email);

        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> LoginByUsernameAsync(string username, string password);
        Task<AuthResult> RefreshTokenAsync(string requestRefreshToken);
        Task<bool> CheckAuthStatusAsync(string token);

        Task<AuthResult> CreateAsync(IdentityUser user, string password);
    }
}
