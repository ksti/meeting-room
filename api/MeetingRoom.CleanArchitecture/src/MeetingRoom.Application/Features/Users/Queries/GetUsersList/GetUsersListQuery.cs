using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Features.Users.Queries.GetUsersList;

/// <summary>
/// 获取用户列表查询
/// </summary>
public class GetUsersListQuery : IRequest<Result<PaginatedList<UserDto>>>
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 10;
    
    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string? SearchKeyword { get; set; }
    
    /// <summary>
    /// 角色
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}
