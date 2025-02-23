using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Exceptions;
using MeetingRoom.Api.Extensions;
using MeetingRoom.Api.Models;
using MeetingRoom.Api.Repositories;

namespace MeetingRoom.Api.Services
{
    public class RoomService(IRoomRepository roomRepository) : IRoomService
    {
        public async Task<RoomModel> CreateRoomAsync(RoomCreateRequest request, string operatorId)
        {
            var newEntity = new RoomEntity
            {
                Name = request.Name,
                Description = request.Description,
                Capacity = request.Capacity,
                Status = request.Status,

            };
            newEntity.SetCreated(operatorId);
            await roomRepository.CreateAsync(newEntity);
            return newEntity.MapToModel();
        }

        public async Task DeleteRoomAsync(string id)
        {
            await roomRepository.DeleteAsync(id);
        }

        public async Task<RoomModel?> GetRoomByIdAsync(string id)
        {
            var entity = await roomRepository.GetByIdAsync(id);
            return entity?.MapToModel();
        }

        public async Task<PagedResult<RoomModel>> SearchAsync(RoomSearchRequest request)
        {
            var result = await roomRepository.SearchAsync(request);
            return new PagedResult<RoomModel>
            {
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total,
                Data = result.Data.Select(u => u.MapToModel())
            };
        }

        public async Task<RoomModel> UpdateRoomAsync(RoomCreateRequest request, string operatorId)
        {
            return await UpdateRoomPartialAsync(request, operatorId);
        }

        public async Task<RoomModel> UpdateRoomPartialAsync(RoomUpdateRequest request, string operatorId)
        {
            var old = await roomRepository.GetByIdAsync(request.Id);
            if (old == null) throw new NotFoundException();

            if (!string.IsNullOrEmpty(request.Name))
                old.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Description))
                old.Description = request.Description;
            if (request.Capacity.HasValue)
                old.Capacity = request.Capacity.Value;
            if (!string.IsNullOrEmpty(request.Status))
                old.Status = request.Status;

            old.SetModified(operatorId);
            await roomRepository.UpdateAsync(old);
            return old.MapToModel();
        }
    }
}
