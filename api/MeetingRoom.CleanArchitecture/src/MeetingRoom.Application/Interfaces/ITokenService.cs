using MeetingRoom.Domain.Aggregates.UserAggregate;

namespace MeetingRoom.Application.Interfaces;

/// <summary>
/// 令牌服务接口
/// 用于生成和验证JWT令牌
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// 生成访问令牌和刷新令牌
    /// </summary>
    /// <param name="user">用户</param>
    /// <returns>访问令牌、刷新令牌和过期时间（秒）</returns>
    Task<(string accessToken, string refreshToken, int expiresIn)> GenerateTokensAsync(User user);
    
    /// <summary>
    /// 验证访问令牌
    /// </summary>
    /// <param name="token">访问令牌</param>
    /// <returns>是否有效</returns>
    Task<bool> ValidateTokenAsync(string token);
    
    /// <summary>
    /// 刷新访问令牌
    /// </summary>
    /// <param name="refreshToken">刷新令牌</param>
    /// <returns>新的访问令牌、刷新令牌和过期时间（秒），如果刷新令牌无效则返回null</returns>
    Task<(string accessToken, string refreshToken, int expiresIn)?> RefreshTokenAsync(string refreshToken);
}
