using MeetingRoom.Domain.Aggregates.UserAggregate;

namespace MeetingRoom.Domain.Interfaces.Repositories;

/// <summary>
/// 用户仓储接口
/// 定义了用户聚合根特有的仓储操作
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户，如果不存在则返回null</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据电子邮件获取用户
    /// </summary>
    /// <param name="email">电子邮件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户，如果不存在则返回null</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="excludeUserId">排除的用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsUsernameExistsAsync(string username, string? excludeUserId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查电子邮件是否已存在
    /// </summary>
    /// <param name="email">电子邮件</param>
    /// <param name="excludeUserId">排除的用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsEmailExistsAsync(string email, string? excludeUserId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取用户的令牌
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>令牌列表</returns>
    Task<List<Token>> GetUserTokensAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据访问令牌获取令牌
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>令牌，如果不存在则返回null</returns>
    Task<Token?> GetTokenByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据刷新令牌获取令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>令牌，如果不存在则返回null</returns>
    Task<Token?> GetTokenByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取用户的设备
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备列表</returns>
    Task<List<Device>> GetUserDevicesAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据设备标识符获取设备
    /// </summary>
    /// <param name="deviceIdentifier">设备标识符</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备，如果不存在则返回null</returns>
    Task<Device?> GetDeviceByIdentifierAsync(string deviceIdentifier, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据多个用户ID获取用户列表
    /// </summary>
    /// <param name="userIds">用户ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    Task<List<User>> GetByIdsAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);
}
