using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Services
{
    public interface IMeetingService
    {
        Task<PagedResult<MeetingModel>> SearchAsync(MeetingSearchRequest request);
        Task<MeetingModel?> GetMeetingByIdAsync(string id);
        Task<ICollection<MeetingModel>> GetMeetingsByTimeAsync(DateTime startTime, DateTime endTime);
        Task<ICollection<MeetingModel>> GetMeetingsByUserIdAsync(string userId, DateTime startTime, DateTime endTime);
        Task<ICollection<MeetingModel>> GetMeetingsByRoomIdAsync(string roomId, DateTime startTime, DateTime endTime);
        Task<MeetingModel> CreateMeetingAsync(MeetingCreateRequest request, string operatorId);
        Task<MeetingModel> UpdateMeetingAsync(MeetingCreateRequest request, string operatorId);
        Task<MeetingModel> UpdateMeetingPartialAsync(MeetingUpdateRequest request, string operatorId);
        Task DeleteMeetingAsync(string id);
    }
}
