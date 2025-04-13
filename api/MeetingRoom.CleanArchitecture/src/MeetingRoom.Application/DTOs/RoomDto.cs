namespace MeetingRoom.Application.DTOs;

/// <summary>
/// 会议室数据传输对象
/// 用于在应用层和表示层之间传输会议室数据
/// </summary>
public class RoomDto
{
    /// <summary>
    /// 会议室ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议室名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议室描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 会议室容量
    /// </summary>
    public int Capacity { get; set; }
    
    /// <summary>
    /// 会议室状态
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// 是否可用
    /// </summary>
    public bool IsAvailable { get; set; }
}
