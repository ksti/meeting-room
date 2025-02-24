using MeetingRoom.Api.Entities;
using MeetingRoom.Api.Exceptions;
using MeetingRoom.Api.Extensions;
using MeetingRoom.Api.Models;
using MeetingRoom.Api.Repositories;

namespace MeetingRoom.Api.Services
{
    public class MeetingService(IMeetingRepository meetingRepository) : IMeetingService
    {
        public async Task<MeetingModel> CreateMeetingAsync(MeetingCreateRequest request, string operatorId)
        {
            var conflictingMeetings = await meetingRepository.GetMeetingsByTimeAsync(request.StarTime, request.EndTime);
            if (conflictingMeetings.Any()) throw new BusinessException("There are conflicting meetings");

            var newEntity = new MeetingEntity
            {
                Name = request.Name,
                Description = request.Description,
                Capacity = request.Capacity,
                StarTime = request.StarTime,
                EndTime = request.EndTime,
                RoomId = request.RoomId,
            };
            newEntity.SetCreated(operatorId);
            await meetingRepository.CreateAsync(newEntity);
            return newEntity.MapToModel();
        }

        public async Task DeleteMeetingAsync(string id)
        {
            await meetingRepository.DeleteAsync(id);
        }

        public async Task<MeetingModel?> GetMeetingByIdAsync(string id)
        {
            var entity = await meetingRepository.GetByIdAsync(id);
            return entity?.MapToModel();
        }

        public async Task<ICollection<MeetingModel>> GetMeetingsByTimeAsync(DateTime startTime, DateTime endTime)
        {
            var result = await meetingRepository.GetMeetingsByTimeAsync(startTime, endTime);
            return result.Select(e => e.MapToModel()).ToList();
        }

        public async Task<ICollection<MeetingModel>> GetMeetingsByUserIdAsync(string userId, DateTime startTime, DateTime endTime)
        {
            var result = await meetingRepository.GetMeetingsByUserIdAsync(userId, startTime, endTime);
            return result.Select(e => e.MapToModel()).ToList();
        }

        public async Task<ICollection<MeetingModel>> GetMeetingsByRoomIdAsync(string roomId, DateTime startTime, DateTime endTime)
        {
            var result = await meetingRepository.GetMeetingsByRoomIdAsync(roomId, startTime, endTime);
            return result.Select(e => e.MapToModel()).ToList();
        }

        public async Task<PagedResult<MeetingModel>> SearchAsync(MeetingSearchRequest request)
        {
            var result = await meetingRepository.SearchAsync(request);
            return new PagedResult<MeetingModel>
            {
                Page = result.Page,
                PageSize = result.PageSize,
                Total = result.Total,
                Data = result.Data.Select(u => u.MapToModel())
            };
        }

        public async Task<MeetingModel> UpdateMeetingAsync(MeetingCreateRequest request, string operatorId)
        {
            return await UpdateMeetingPartialAsync(request, operatorId);
        }

        public async Task<MeetingModel> UpdateMeetingPartialAsync(MeetingUpdateRequest request, string operatorId)
        {
            var old = await meetingRepository.GetByIdAsync(request.Id);
            if (old == null) throw new NotFoundException();

            if (!string.IsNullOrEmpty(request.Name))
                old.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Description))
                old.Description = request.Description;
            if (request.Capacity.HasValue)
                old.Capacity = request.Capacity.Value;
            if (request.StarTime.HasValue)
                old.StarTime = request.StarTime.Value;
            if (request.EndTime.HasValue)
                old.EndTime = request.EndTime.Value;
            if (!string.IsNullOrEmpty(request.RoomId))
                old.RoomId = request.RoomId;
            if (!string.IsNullOrEmpty(request.Status))
                old.Status = request.Status;

            old.SetModified(operatorId);
            await meetingRepository.UpdateAsync(old);
            return old.MapToModel();
        }
    }
}
