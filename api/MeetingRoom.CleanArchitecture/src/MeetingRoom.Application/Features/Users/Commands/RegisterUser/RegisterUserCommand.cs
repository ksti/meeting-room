using MediatR;
using MeetingRoom.Application.Common;

namespace MeetingRoom.Application.Features.Users.Commands.RegisterUser;

/// <summary>
/// 用户注册命令
/// </summary>
public class RegisterUserCommand : IRequest<Result<string>>
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 电子邮件
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
    
    /// <summary>
    /// 姓
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// 名
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// 联系方式
    /// </summary>
    public string? Contact { get; set; }
}
