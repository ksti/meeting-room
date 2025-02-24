namespace MeetingRoom.Api.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public string UserId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public UserModel? User { get; set; }
        public DeviceModel? Device { get; set; }
    }
}
