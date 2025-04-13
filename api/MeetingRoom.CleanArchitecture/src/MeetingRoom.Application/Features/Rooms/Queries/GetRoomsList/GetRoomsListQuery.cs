using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Features.Rooms.Queries.GetRoomsList;

/// <summary>
/// 获取会议室列表查询
/// </summary>
public class GetRoomsListQuery : IRequest<Result<PaginatedList<RoomDto>>>
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
    /// 状态
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// 最小容量
    /// </summary>
    public int? MinCapacity { get; set; }
    
    /// <summary>
    /// 最大容量
    /// </summary>
    public int? MaxCapacity { get; set; }
    
    /// <summary>
    /// 是否只显示可用的会议室
    /// </summary>
    public bool? OnlyAvailable { get; set; }
}
