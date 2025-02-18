using MeetingRoom.Api.Entities;

namespace MeetingRoom.Api.Repositories
{
    public interface IDeviceRepository : IRepository<DeviceEntity>
    {
        Task RemoveUserIdAsync();
    }
}
