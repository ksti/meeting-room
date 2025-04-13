using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Features.Users.Commands.LoginUser;

/// <summary>
/// 用户登录命令
/// </summary>
public class LoginUserCommand : IRequest<Result<LoginResponseDto>>
{
    /// <summary>
    /// 用户名或电子邮件
    /// </summary>
    public string UsernameOrEmail { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 设备标识符
    /// </summary>
    public string? DeviceIdentifier { get; set; }
    
    /// <summary>
    /// 设备名称
    /// </summary>
    public string? DeviceName { get; set; }
    
    /// <summary>
    /// 平台
    /// </summary>
    public string? Platform { get; set; }
    
    /// <summary>
    /// 操作系统
    /// </summary>
    public string? OperatingSystem { get; set; }
    
    /// <summary>
    /// 操作系统版本
    /// </summary>
    public string? OsVersion { get; set; }
}

/// <summary>
/// 登录响应数据传输对象
/// </summary>
public class LoginResponseDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 电子邮件
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 访问令牌
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// 令牌类型
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
    
    /// <summary>
    /// 访问令牌过期时间（秒）
    /// </summary>
    public int ExpiresIn { get; set; }
    
    /// <summary>
    /// 用户信息
    /// </summary>
    public UserDto User { get; set; } = new UserDto();
}
