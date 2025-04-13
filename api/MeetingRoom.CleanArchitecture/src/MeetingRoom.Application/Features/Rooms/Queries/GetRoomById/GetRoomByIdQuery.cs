using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Features.Rooms.Queries.GetRoomById;

/// <summary>
/// 根据ID获取会议室查询
/// </summary>
public class GetRoomByIdQuery : IRequest<Result<RoomDto>>
{
    /// <summary>
    /// 会议室ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
