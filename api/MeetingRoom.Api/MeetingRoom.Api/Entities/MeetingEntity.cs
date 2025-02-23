using MeetingRoom.Api.Common;
using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Entities
{
    public class MeetingEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RoomId { get; set; } = null!;
        public string Status { get; set; } = MeetingStatus.Scheduled.GetDisplayName();
        public virtual RoomEntity? Room { get; set; }
        public virtual ICollection<UserEntity> Participants { get; set; } = [];
    }
}
