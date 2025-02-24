using MeetingRoom.Api.Common;
using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Entities
{
    public class RoomEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Status { get; set; } = RoomStatus.Idle.GetDisplayName();
        public virtual ICollection<MeetingEntity> Meetings { get; set; } = [];
    }
}
