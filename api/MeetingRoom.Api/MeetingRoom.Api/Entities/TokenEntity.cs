using System.ComponentModel.DataAnnotations;
using MeetingRoom.Api.Common;

namespace MeetingRoom.Api.Entities
{
    public class TokenEntity : BaseEntity
    {
        [MaxLength(36)]
        public string UserId { get; set; } = string.Empty;
        [MaxLength(200)]
        public string AccessToken { get; set; } = string.Empty;
        [MaxLength(200)]
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        [MaxLength(20)]
        public string TokenType { get; set; } = "Bearer";

        // Navigation properties
        public virtual UserEntity? User { get; set; }
    }
}
