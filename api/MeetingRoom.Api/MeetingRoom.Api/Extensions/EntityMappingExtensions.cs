using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Extensions
{
    public static class EntityMappingExtensions
    {
        public static UserModel MapToModel(this UserEntity entity)
        {
            return new UserModel
            {
                Id = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Username = entity.Username,
                Role = entity.Role,
                Status = entity.Status,
                Devices = entity.Devices.Select(MapToModel).ToList(),
                Meetings = entity.Meetings.Select(MapToModel).ToList(),
            };
        }
        public static TokenModel MapToModel(this TokenEntity entity)
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
        public static DeviceModel MapToModel(this DeviceEntity entity)
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
                Token = entity.Token?.MapToModel(),
            };
        }
        public static MeetingModel MapToModel(this MeetingEntity entity)
        {
            return new MeetingModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Capacity = entity.Capacity,
                StarTime = entity.StarTime,
                EndTime = entity.EndTime,
                RoomId = entity.RoomId,
                Status = entity.Status,
                Room = entity.Room?.MapToModel(),
                Participants = entity.Participants.Select(MapToModel).ToList(),
            };
        }
        public static RoomModel MapToModel(this RoomEntity entity)
        {
            return new RoomModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Capacity = entity.Capacity,
                Status = entity.Status,
                Meetings = entity.Meetings.Select(MapToModel).ToList(),
            };
        }
    }
}
