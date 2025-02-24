using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Extensions
{
    public static class EntityMappingExtensions
    {
        public static UserModel MapToModel(this UserEntity entity, bool? isLoadReference = true)
        {
            return new UserModel
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Username = entity.Username,
                Contact = entity.Contact,
                Avatar = entity.Avatar,
                Role = entity.Role,
                Status = entity.Status,
                Devices = isLoadReference.HasValue && isLoadReference.Value ? entity.Devices.Select(e => MapToModel(e, false)).ToList() : [],
                Tokens = isLoadReference.HasValue && isLoadReference.Value ? entity.Tokens.Select(e => MapToModel(e, false)).ToList() : [],
                Meetings = isLoadReference.HasValue && isLoadReference.Value ? entity.Meetings.Select(e => MapToModel(e, false)).ToList() : [],
            };
        }
        public static TokenModel MapToModel(this TokenEntity entity, bool? isLoadReference = true)
        {
            return new TokenModel
            {
                AccessToken = entity.AccessToken,
                RefreshToken = entity.RefreshToken,
                AccessTokenExpiresAt = entity.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = entity.RefreshTokenExpiresAt,
                TokenType = entity.TokenType,
            };
        }
        public static DeviceModel MapToModel(this DeviceEntity entity, bool? isLoadReference = true)
        {
            return new DeviceModel
            {
                Id = entity.Id,
                TokenId = entity.TokenId,
                DeviceIdentifier = entity.DeviceIdentifier,
                DeviceName = entity.DeviceName,
                Platform = entity.Platform,
                OperatingSystem = entity.OperatingSystem,
                OsVersion = entity.OsVersion,
                AppVersion = entity.AppVersion,
                Browser = entity.Browser,
                BrowserVersion = entity.BrowserVersion,
                LastIpAddress = entity.LastIpAddress,
                LastLoginAt = entity.LastLoginAt,
                Status = entity.Status,
                Token = isLoadReference.HasValue && isLoadReference.Value ? entity.Token?.MapToModel(false) : null,
            };
        }
        public static MeetingModel MapToModel(this MeetingEntity entity, bool? isLoadReference = true)
        {
            return new MeetingModel
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Capacity = entity.Capacity,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                OrganizerId = entity.OrganizerId,
                RoomId = entity.RoomId,
                Status = entity.Status,
                Organizer = isLoadReference.HasValue && isLoadReference.Value ? entity.Organizer?.MapToModel(false) : null,
                Room = isLoadReference.HasValue && isLoadReference.Value ? entity.Room?.MapToModel(false) : null,
                Attendees = isLoadReference.HasValue && isLoadReference.Value ? entity.Attendees.Select(e => MapToModel(e, false)).ToList() : [],
            };
        }
        public static RoomModel MapToModel(this RoomEntity entity, bool? isLoadReference = true)
        {
            return new RoomModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Capacity = entity.Capacity,
                Status = entity.Status,
                Meetings = isLoadReference.HasValue && isLoadReference.Value ? entity.Meetings.Select(e => MapToModel(e, false)).ToList() : [],
            };
        }
    }
}
