using System.ComponentModel.DataAnnotations;
using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Models
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = UserRole.User.GetDisplayName();
        public string Status { get; set; } = UserStatus.Active.GetDisplayName();
        public ICollection<DeviceModel> Devices { get; set; } = [];
        public ICollection<MeetingModel> Meetings { get; set; } = [];
    }
    public class UserUpdateRequest
    {
        public string Id { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
    }
    public class UserCreateRequest : UserUpdateRequest
    {
        [Required]
        [EmailAddress]
        public new string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(3)]
        public new string Username { get; set; } = string.Empty;
        public new string Role { get; set; } = UserRole.User.GetDisplayName();
        [Required]
        [MinLength(6)]
        public new string Password { get; set; } = string.Empty;
        public new string Status { get; set; } = UserStatus.Active.GetDisplayName();
        [Required] public DeviceInfo DeviceInfo { get; set; } = null!;
    }
    public class UserSearchRequest : BaseSearchRequest
    {
    }
}
