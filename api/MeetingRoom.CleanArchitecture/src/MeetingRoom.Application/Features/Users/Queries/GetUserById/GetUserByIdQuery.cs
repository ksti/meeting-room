using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// 根据ID获取用户查询
/// </summary>
public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
