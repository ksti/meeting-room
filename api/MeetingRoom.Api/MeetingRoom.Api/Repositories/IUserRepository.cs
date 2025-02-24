using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Repositories
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<PagedResult<UserEntity>> SearchAsync(UserSearchRequest searchRequest);
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetByRefreshTokenAsync(string refreshToken);
    }
}
