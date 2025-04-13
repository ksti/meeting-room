using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Domain.Aggregates.UserAggregate;

/// <summary>
/// 令牌实体
/// 表示用户的认证令牌，如JWT令牌
/// 作为实体，Token有唯一标识，但生命周期由User聚合根管理
/// </summary>
public class Token : AuditableEntity
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; private set; }
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string? RefreshToken { get; private set; }
    
    /// <summary>
    /// 令牌类型
    /// </summary>
    public string TokenType { get; private set; }
    
    /// <summary>
    /// 访问令牌过期时间
    /// </summary>
    public DateTime AccessTokenExpiresAt { get; private set; }
    
    /// <summary>
    /// 刷新令牌过期时间
    /// </summary>
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    
    /// <summary>
    /// 是否已吊销
    /// </summary>
    public bool IsRevoked { get; private set; }
    
    /// <summary>
    /// 吊销时间
    /// </summary>
    public DateTime? RevokedAt { get; private set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; private set; }
    
    // 导航属性，由ORM框架使用
    public User? User { get; private set; }
    
    // 私有构造函数，防止直接创建实例
    private Token() 
    { 
        AccessToken = string.Empty;
        TokenType = "Bearer";
        AccessTokenExpiresAt = DateTime.MinValue;
        UserId = string.Empty;
        IsRevoked = true;
    }
    
    /// <summary>
    /// 创建新令牌
    /// </summary>
    /// <param name="accessToken">访问令牌</param>
    /// <param name="tokenType">令牌类型</param>
    /// <param name="accessTokenExpiresAt">访问令牌过期时间</param>
    /// <param name="userId">用户ID</param>
    /// <param name="refreshToken">刷新令牌</param>
    /// <param name="refreshTokenExpiresAt">刷新令牌过期时间</param>
    /// <returns>令牌实体</returns>
    public static Token Create(
        string accessToken,
        string tokenType,
        DateTime accessTokenExpiresAt,
        string userId,
        string? refreshToken = null,
        DateTime? refreshTokenExpiresAt = null)
    {
        return new Token
        {
            AccessToken = accessToken,
            TokenType = tokenType,
            AccessTokenExpiresAt = accessTokenExpiresAt,
            UserId = userId,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            IsRevoked = false
        };
    }
    
    /// <summary>
    /// 吊销令牌
    /// </summary>
    public void Revoke()
    {
        if (!IsRevoked)
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }
    }
    
    /// <summary>
    /// 更新刷新令牌
    /// </summary>
    /// <param name="refreshToken">新的刷新令牌</param>
    /// <param name="refreshTokenExpiresAt">新的刷新令牌过期时间</param>
    public void UpdateRefreshToken(string refreshToken, DateTime refreshTokenExpiresAt)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;
    }
    
    /// <summary>
    /// 检查访问令牌是否已过期
    /// </summary>
    /// <returns>是否已过期</returns>
    public bool IsAccessTokenExpired()
    {
        return DateTime.UtcNow >= AccessTokenExpiresAt;
    }
    
    /// <summary>
    /// 检查刷新令牌是否已过期
    /// </summary>
    /// <returns>是否已过期</returns>
    public bool IsRefreshTokenExpired()
    {
        return RefreshTokenExpiresAt.HasValue && DateTime.UtcNow >= RefreshTokenExpiresAt.Value;
    }
    
    /// <summary>
    /// 检查令牌是否有效
    /// </summary>
    /// <returns>是否有效</returns>
    public bool IsValid()
    {
        return !IsRevoked && !IsAccessTokenExpired();
    }
}
