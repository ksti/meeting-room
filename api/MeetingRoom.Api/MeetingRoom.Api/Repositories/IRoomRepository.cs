using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Repositories
{
    public interface IRoomRepository : IRepository<RoomEntity>
    {
        Task<PagedResult<RoomEntity>> SearchAsync(RoomSearchRequest searchRequest);
    }
}
