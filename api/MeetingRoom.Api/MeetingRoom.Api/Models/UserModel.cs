using System.ComponentModel.DataAnnotations;
using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Models
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = UserRole.User.GetDisplayName();
    }
    public class UserUpdateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Password { get; set; }
    }
    public class UserRegisterRequest : UserUpdateRequest
    {
        [Required]
        public new string Email { get; set; } = string.Empty;
        public new string Role { get; set; } = UserRole.User.GetDisplayName();
        [Required]
        [MinLength(6)]
        public new string Password { get; set; } = string.Empty;
    }
}
