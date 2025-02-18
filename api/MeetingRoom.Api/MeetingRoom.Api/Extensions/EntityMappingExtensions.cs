using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Extensions
{
    public static class EntityMappingExtensions
    {
        public static UserModel MapToModel(this UserEntity userEntity)
        {
            return new UserModel
            {
                Id = userEntity.Id,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                Username = userEntity.Username,
                Role = userEntity.Role,
            };
        }
    }
}
