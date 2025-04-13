namespace MeetingRoom.Application.DTOs;

/// <summary>
/// 会议详情数据传输对象
/// 用于在应用层和表示层之间传输详细的会议数据
/// </summary>
public class MeetingDetailDto
{
    /// <summary>
    /// 会议ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议标题
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// 会议状态
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议室
    /// </summary>
    public RoomDto? Room { get; set; }
    
    /// <summary>
    /// 组织者
    /// </summary>
    public UserDto? Organizer { get; set; }
    
    /// <summary>
    /// 参与者列表
    /// </summary>
    public List<UserDto> Participants { get; set; } = new List<UserDto>();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// 创建者ID
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// 最后更新者ID
    /// </summary>
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// 当前参与者数量
    /// </summary>
    public int ParticipantCount => Participants.Count;
    
    /// <summary>
    /// 会议时长（分钟）
    /// </summary>
    public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;
    
    /// <summary>
    /// 是否是当前用户组织的会议
    /// </summary>
    public bool IsCurrentUserOrganizer { get; set; }
    
    /// <summary>
    /// 当前用户是否是参与者
    /// </summary>
    public bool IsCurrentUserParticipant { get; set; }
}
