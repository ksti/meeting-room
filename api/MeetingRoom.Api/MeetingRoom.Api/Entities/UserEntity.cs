using MeetingRoom.Api.Common;
using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Entities
{
    public class UserEntity : BaseEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Contact { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public string Role { get; set; } = UserRole.User.GetDisplayName();
        public string Status { get; set; } = UserStatus.Active.GetDisplayName();
        public virtual ICollection<TokenEntity> Tokens { get; set; } = [];
        public virtual ICollection<DeviceEntity> Devices { get; set; } = [];
        public virtual ICollection<MeetingEntity> Meetings { get; set; } = [];
    }
}
