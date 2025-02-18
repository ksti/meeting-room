using MeetingRoom.Api.Entities;

namespace MeetingRoom.Api.Repositories
{
    public interface ITokenRepository : IRepository<TokenEntity>
    {
        Task RemoveUserIdAsync();
    }
}
