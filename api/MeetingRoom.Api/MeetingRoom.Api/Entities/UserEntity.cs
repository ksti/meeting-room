using MeetingRoom.Api.Common;

namespace MeetingRoom.Api.Entities
{
    public class UserEntity : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public virtual ICollection<TokenEntity> Tokens { get; set; } = [];
        public virtual ICollection<DeviceEntity> Devices { get; set; } = [];
    }
}
