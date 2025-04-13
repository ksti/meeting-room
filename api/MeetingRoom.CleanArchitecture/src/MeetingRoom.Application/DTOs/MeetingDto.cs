namespace MeetingRoom.Application.DTOs;

/// <summary>
/// 会议数据传输对象
/// 用于在应用层和表示层之间传输会议数据
/// </summary>
public class MeetingDto
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
    public string? Description { get; set; }
    
    /// <summary>
    /// 会议容量
    /// </summary>
    public int Capacity { get; set; }
    
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// 组织者ID
    /// </summary>
    public string OrganizerId { get; set; } = string.Empty;
    
    /// <summary>
    /// 组织者名称
    /// </summary>
    public string? OrganizerName { get; set; }
    
    /// <summary>
    /// 会议室ID
    /// </summary>
    public string RoomId { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议室名称
    /// </summary>
    public string? RoomName { get; set; }
    
    /// <summary>
    /// 会议状态
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// 参与者列表
    /// </summary>
    public List<AttendeeDto> Attendees { get; set; } = new List<AttendeeDto>();
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// 当前参与者数量
    /// </summary>
    public int AttendeeCount => Attendees.Count;
    
    /// <summary>
    /// 剩余容量
    /// </summary>
    public int RemainingCapacity => Capacity - AttendeeCount;
    
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
    public bool IsCurrentUserAttendee { get; set; }
}

/// <summary>
/// 参与者数据传输对象
/// </summary>
public class AttendeeDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// 全名
    /// </summary>
    public string? FullName { get; set; }
    
    /// <summary>
    /// 电子邮件
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }
}
