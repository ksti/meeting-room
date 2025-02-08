using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<PagedResult<User>> SearchAsync(UserSearchRequest userSearch);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}
