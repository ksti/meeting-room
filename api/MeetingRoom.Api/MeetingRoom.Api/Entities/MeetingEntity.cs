using System.ComponentModel.DataAnnotations.Schema;
using MeetingRoom.Api.Common;
using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Entities
{
    public class MeetingEntity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string OrganizerId { get; set; } = null!;
        public string RoomId { get; set; } = null!;
        public string Status { get; set; } = MeetingStatus.Scheduled.GetDisplayName();
        public virtual RoomEntity? Room { get; set; }
        [NotMapped]
        public virtual UserEntity? Organizer { get; set; }
        public virtual ICollection<UserEntity> Attendees { get; set; } = [];
    }
}
