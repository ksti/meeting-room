using MeetingRoom.Api.Entities;

namespace MeetingRoom.Api.Repositories
{
    public interface ITokenRepository : IRepository<TokenEntity>
    {
        Task<TokenEntity?> GetByRefreshTokenAsync(string refreshToken);
        Task RemoveUserIdAsync();
    }
}
