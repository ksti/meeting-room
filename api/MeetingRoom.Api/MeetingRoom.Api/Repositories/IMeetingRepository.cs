using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Repositories
{
    public interface IMeetingRepository : IRepository<MeetingEntity>
    {
        Task<PagedResult<MeetingEntity>> SearchAsync(MeetingSearchRequest searchRequest);
        Task<ICollection<MeetingEntity>> GetMeetingsByTimeAsync(DateTime startTime, DateTime endTime);
        Task<ICollection<MeetingEntity>> GetMeetingsByUserIdAsync(string userId, DateTime startTime, DateTime endTime);
        Task<ICollection<MeetingEntity>> GetMeetingsByRoomIdAsync(string roomId, DateTime startTime, DateTime endTime);
    }
}
