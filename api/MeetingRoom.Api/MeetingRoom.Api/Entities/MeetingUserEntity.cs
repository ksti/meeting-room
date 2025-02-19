using MeetingRoom.Api.Common;

namespace MeetingRoom.Api.Entities
{
    public class MeetingUserEntity : BaseEntity
    {
        public string MeetingId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public virtual UserEntity User { get; set; } = null!;
        public virtual MeetingEntity Meeting { get; set; } = null!;
    }
}
