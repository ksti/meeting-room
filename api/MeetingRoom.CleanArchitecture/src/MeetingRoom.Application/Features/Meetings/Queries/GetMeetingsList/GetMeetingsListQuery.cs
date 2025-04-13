using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Features.Meetings.Queries.GetMeetingsList;

/// <summary>
/// 获取会议列表查询
/// </summary>
public class GetMeetingsListQuery : IRequest<Result<PaginatedList<MeetingDto>>>
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
    /// 会议室ID
    /// </summary>
    public string? RoomId { get; set; }
    
    /// <summary>
    /// 组织者ID
    /// </summary>
    public string? OrganizerId { get; set; }
    
    /// <summary>
    /// 参与者ID
    /// </summary>
    public string? ParticipantId { get; set; }
    
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}
