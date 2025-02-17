using MeetingRoom.Api.Common;

namespace MeetingRoom.Api.Entities
{
    public class TokenEntity : BaseEntity
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public string UserId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public virtual UserEntity User { get; set; } = null!;
        public virtual DeviceEntity Device { get; set; } = null!;
    }
}
