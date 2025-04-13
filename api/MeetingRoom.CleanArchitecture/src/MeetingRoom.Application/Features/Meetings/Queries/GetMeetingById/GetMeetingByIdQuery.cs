using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;

namespace MeetingRoom.Application.Features.Meetings.Queries.GetMeetingById;

/// <summary>
/// 根据ID获取会议查询
/// </summary>
public class GetMeetingByIdQuery : IRequest<Result<MeetingDetailDto>>
{
    /// <summary>
    /// 会议ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
