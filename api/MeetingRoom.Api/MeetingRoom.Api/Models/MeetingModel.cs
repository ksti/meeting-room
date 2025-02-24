using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Api.Models
{
    public class MeetingModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RoomId { get; set; } = null!;
        public string Status { get; set; } = MeetingStatus.Scheduled.GetDisplayName();
        public virtual RoomModel? Room { get; set; }
        public virtual ICollection<UserModel> Participants { get; set; } = [];
    }
    public class MeetingUpdateRequest
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Capacity { get; set; }
        public DateTime? StarTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? RoomId { get; set; }
        public string? Status { get; set; }
    }
    public class MeetingCreateRequest : MeetingUpdateRequest
    {
        [Required]
        public new string Name { get; set; } = string.Empty;
        [Range(1, 10000)]
        public new int Capacity { get; set; }
        [Required]
        public new DateTime StarTime { get; set; }
        [Required]
        public new DateTime EndTime { get; set; }
        [Required]
        public new string RoomId { get; set; } = string.Empty;
        public new string Status { get; set; } = MeetingStatus.Scheduled.GetDisplayName();
    }
    public class MeetingSearchRequest : BaseSearchRequest
    {
    }
}
