using MeetingRoom.Api.Data;
using MeetingRoom.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ApplicationDbContext _context;

        public DeviceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceEntity?> GetByIdAsync(string id)
        {
            return await _context.Devices.FindAsync(id);
        }

        public async Task<DeviceEntity> CreateAsync(DeviceEntity deviceEntity)
        {
            _context.Devices.Add(deviceEntity);
            await _context.SaveChangesAsync();
            return deviceEntity;
        }

        public async Task UpdateAsync(DeviceEntity deviceEntity)
        {
            _context.Entry(deviceEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var deviceEntity = await _context.Devices.FindAsync(id);
            if (deviceEntity != null)
            {
                _context.Devices.Remove(deviceEntity);
            }
        }

        public async Task RemoveUserIdAsync()
        {
            _context.Devices.RemoveRange(_context.Devices);
        }
    }
}
