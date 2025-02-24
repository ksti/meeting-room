using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Services
{
    public interface IRoomService
    {
        Task<PagedResult<RoomModel>> SearchAsync(RoomSearchRequest request);
        Task<RoomModel?> GetRoomByIdAsync(string id);
        Task<RoomModel> CreateRoomAsync(RoomCreateRequest request, string operatorId);
        Task<RoomModel> UpdateRoomAsync(RoomCreateRequest request, string operatorId);
        Task<RoomModel> UpdateRoomPartialAsync(RoomUpdateRequest request, string operatorId);
        Task DeleteRoomAsync(string id);
    }
}
