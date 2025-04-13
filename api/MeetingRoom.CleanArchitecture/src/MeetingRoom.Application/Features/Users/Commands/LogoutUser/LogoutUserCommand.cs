using MediatR;
using MeetingRoom.Application.Common;

namespace MeetingRoom.Application.Features.Users.Commands.LoginUser;

/// <summary>
/// 用户注销命令
/// </summary>
public class LogoutUserCommand : IRequest<Result<bool>>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string? UserId { get; set; }
}
