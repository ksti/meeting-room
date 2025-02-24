namespace MeetingRoom.Api.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpiresMinutes { get; set; } = 60;
        public int RefreshTokenExpiryDays { get; set; } = 7;
        public int MaxAllowedDevices { get; set; } = 5;
    }
}
