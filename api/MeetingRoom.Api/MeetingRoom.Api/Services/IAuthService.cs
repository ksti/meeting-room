using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Services
{
    public interface IAuthService
    {
        Task<bool> VerifyPasswordAsync(string email, string password);
        Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<bool> LogoutAsync(string email);

        Task<AuthResult> LoginAsync(string email, string requestPassword);
        Task<AuthResult> LoginByUsernameAsync(string username, string requestPassword);
        Task<AuthResult> RefreshTokenAsync(string requestRefreshToken);
        Task<bool> CheckAuthStatusAsync(string token);
    }
}
