using MediatR;
using MeetingRoom.Application.Common;

namespace MeetingRoom.Application.Features.Meetings.Commands.CreateMeeting;

/// <summary>
/// 创建会议命令
/// </summary>
public class CreateMeetingCommand : IRequest<Result<string>>
{
    /// <summary>
    /// 会议标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 会议室ID
    /// </summary>
    public string RoomId { get; set; } = string.Empty;
    
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// 会议容量
    /// </summary>
    public int Capacity { get; set; } = 10;
    
    /// <summary>
    /// 参与者ID列表
    /// </summary>
    public List<string> ParticipantIds { get; set; } = new List<string>();
}
