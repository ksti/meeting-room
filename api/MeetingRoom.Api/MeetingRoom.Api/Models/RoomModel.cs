using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Api.Models
{
    public class RoomModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = RoomStatus.Idle.GetDisplayName();
        public ICollection<MeetingModel> Meetings { get; set; } = [];
    }
    public class RoomUpdateRequest
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Capacity { get; set; }
        public string? Status { get; set; }
    }
    public class RoomCreateRequest : RoomUpdateRequest
    {
        [Required]
        public new string Name { get; set; } = string.Empty;
        [Range(1, 10000)]
        public new int Capacity { get; set; }
        public new string Status { get; set; } = RoomStatus.Idle.GetDisplayName();
    }
    public class RoomSearchRequest : BaseSearchRequest
    {
    }
}
