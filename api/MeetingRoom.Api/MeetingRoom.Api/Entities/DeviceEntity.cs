﻿using MeetingRoom.Api.Common;
using MeetingRoom.Api.Enums;
using Microsoft.OpenApi.Extensions;

namespace MeetingRoom.Api.Entities
{
    public class DeviceEntity : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string TokenId { get; set; } = string.Empty;
        public string DeviceIdentifier { get; set; } = string.Empty;

        public string? DeviceName { get; set; }

        public string? Platform { get; set; }

        public string? OperatingSystem { get; set; }

        public string? OsVersion { get; set; }

        public string? AppVersion { get; set; }

        public string? Browser { get; set; }

        public string? BrowserVersion { get; set; }

        public string? LastIpAddress { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public string Status { get; set; } = DeviceStatus.Active.GetDisplayName(); // active, inactive, blocked

        public bool IsActive => Status == DeviceStatus.Active.GetDisplayName();

        // Navigation properties
        public virtual UserEntity? User { get; set; }
        public virtual TokenEntity? Token { get; set; }
    }
}
