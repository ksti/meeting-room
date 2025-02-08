namespace MeetingRoom.Api.Models
{
    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresIn { get; set; }
        public UserModel? User { get; set; }
    }
}
