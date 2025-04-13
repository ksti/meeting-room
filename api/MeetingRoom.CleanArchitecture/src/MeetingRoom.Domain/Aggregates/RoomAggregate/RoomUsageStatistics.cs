using System;

namespace MeetingRoom.Domain.Aggregates.RoomAggregate;

/// <summary>
/// 会议室使用统计值对象
/// </summary>
public class RoomUsageStatistics
{
    /// <summary>
    /// 会议室ID
    /// </summary>
    public string RoomId { get; set; } = string.Empty;
    
    /// <summary>
    /// 统计开始日期
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// 统计结束日期
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// 会议总数
    /// </summary>
    public int TotalMeetings { get; set; }
    
    /// <summary>
    /// 总使用时长（分钟）
    /// </summary>
    public int TotalDurationMinutes { get; set; }
    
    /// <summary>
    /// 平均会议时长（分钟）
    /// </summary>
    public int AverageDurationMinutes { get; set; }
}
