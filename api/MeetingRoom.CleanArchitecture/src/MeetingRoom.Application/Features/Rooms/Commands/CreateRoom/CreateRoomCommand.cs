using MediatR;
using MeetingRoom.Application.Common;

namespace MeetingRoom.Application.Features.Rooms.Commands.CreateRoom;

/// <summary>
/// 创建会议室命令
/// </summary>
public class CreateRoomCommand : IRequest<Result<string>>
{
    /// <summary>
    /// 会议室名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议室描述
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// 会议室容量
    /// </summary>
    public int Capacity { get; set; }
}
