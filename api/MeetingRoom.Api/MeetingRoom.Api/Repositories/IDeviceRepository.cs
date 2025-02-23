using MeetingRoom.Api.Entities;

namespace MeetingRoom.Api.Repositories
{
    public interface IDeviceRepository : IRepository<DeviceEntity>
    {
        Task<DeviceEntity?> GetByDeviceIdentifierAsync(string deviceIdentifier, string? userId = null);
        Task RemoveUserIdAsync();
    }
}
