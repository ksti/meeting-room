using Microsoft.EntityFrameworkCore;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.Interfaces.Repositories;
using MeetingRoom.Domain.ValueObjects;

namespace MeetingRoom.Infrastructure.Persistence.Repositories;

/// <summary>
/// 用户仓储实现
/// 实现IUserRepository接口，提供用户相关的数据访问功能
/// </summary>
public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    /// <summary>
    /// 获取用户集合，包括相关实体
    /// </summary>
    protected override IQueryable<User> EntitySet => _dbContext.Users
        .Include(u => u.Devices)
        .Include(u => u.Tokens);
    
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户，如果不存在则返回null</returns>
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
    
    /// <summary>
    /// 根据电子邮件获取用户
    /// </summary>
    /// <param name="email">电子邮件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户，如果不存在则返回null</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
    }
    
    /// <summary>
    /// 检查用户名是否存在
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="excludeUserId">排除的用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    public async Task<bool> IsUsernameExistsAsync(string username, string? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users.AsQueryable();
        
        if (!string.IsNullOrEmpty(excludeUserId))
        {
            query = query.Where(u => u.Id != excludeUserId);
        }
        
        return await query.AnyAsync(u => u.Username == username, cancellationToken);
    }
    
    /// <summary>
    /// 检查电子邮件是否存在
    /// </summary>
    /// <param name="email">电子邮件</param>
    /// <param name="excludeUserId">排除的用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    public async Task<bool> IsEmailExistsAsync(string email, string? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users.AsQueryable();
        
        if (!string.IsNullOrEmpty(excludeUserId))
        {
            query = query.Where(u => u.Id != excludeUserId);
        }
        
        return await query.AnyAsync(u => u.Email.Value == email, cancellationToken);
    }
    
    /// <summary>
    /// 根据刷新令牌获取用户
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户，如果不存在则返回null</returns>
    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.RefreshToken == refreshToken && t.RefreshTokenExpiresAt > DateTime.UtcNow), cancellationToken);
    }
    
    /// <summary>
    /// 根据设备标识符获取用户
    /// </summary>
    /// <param name="deviceIdentifier">设备标识符</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户，如果不存在则返回null</returns>
    public async Task<User?> GetByDeviceIdentifierAsync(string deviceIdentifier, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .FirstOrDefaultAsync(u => u.Devices.Any(d => d.DeviceInfo.DeviceIdentifier == deviceIdentifier), cancellationToken);
    }
    
    /// <summary>
    /// 获取用户的令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>令牌列表</returns>
    public async Task<List<Token>> GetUserTokensAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await EntitySet
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            
        return user?.Tokens.ToList() ?? new List<Token>();
    }
    
    /// <summary>
    /// 根据访问令牌获取令牌
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>令牌，如果不存在则返回null</returns>
    public async Task<Token?> GetTokenByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var user = await EntitySet
            .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.AccessToken == accessToken), cancellationToken);
            
        return user?.Tokens.FirstOrDefault(t => t.AccessToken == accessToken);
    }
    
    /// <summary>
    /// 根据刷新令牌获取令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>令牌，如果不存在则返回null</returns>
    public async Task<Token?> GetTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await EntitySet
            .FirstOrDefaultAsync(u => u.Tokens.Any(t => t.RefreshToken == refreshToken), cancellationToken);
            
        return user?.Tokens.FirstOrDefault(t => t.RefreshToken == refreshToken);
    }
    
    /// <summary>
    /// 获取用户的设备
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备列表</returns>
    public async Task<List<Device>> GetUserDevicesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await EntitySet
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            
        return user?.Devices.ToList() ?? new List<Device>();
    }
    
    /// <summary>
    /// 根据设备标识符获取设备
    /// </summary>
    /// <param name="deviceIdentifier">设备标识符</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备，如果不存在则返回null</returns>
    public async Task<Device?> GetDeviceByIdentifierAsync(string deviceIdentifier, CancellationToken cancellationToken = default)
    {
        var user = await GetByDeviceIdentifierAsync(deviceIdentifier, cancellationToken);
        
        return user?.Devices.FirstOrDefault(d => d.DeviceInfo.DeviceIdentifier == deviceIdentifier);
    }
    
    /// <summary>
    /// 根据多个用户ID获取用户列表
    /// </summary>
    /// <param name="userIds">用户ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    public override async Task<List<User>> GetByIdsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }
}
