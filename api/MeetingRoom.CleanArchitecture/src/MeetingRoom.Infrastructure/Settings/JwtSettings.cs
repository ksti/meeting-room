namespace MeetingRoom.Infrastructure.Settings;

/// <summary>
/// JWT配置
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 密钥
    /// </summary>
    public string Secret { get; set; } = string.Empty;
    
    /// <summary>
    /// 发行者
    /// </summary>
    public string Issuer { get; set; } = string.Empty;
    
    /// <summary>
    /// 接收者
    /// </summary>
    public string Audience { get; set; } = string.Empty;
    
    /// <summary>
    /// 过期时间（分钟）
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;
    
    /// <summary>
    /// 刷新令牌过期时间（天）
    /// </summary>
    public int RefreshTokenExpirationInDays { get; set; } = 7;
}
