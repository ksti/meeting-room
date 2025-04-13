using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Domain.Enums;

/// <summary>
/// 会议状态枚举
/// </summary>
public enum MeetingStatus
{
    /// <summary>
    /// 已安排
    /// </summary>
    [Display(Name = "Scheduled")]
    Scheduled,
    
    /// <summary>
    /// 进行中
    /// </summary>
    [Display(Name = "InProgress")]
    InProgress,
    
    /// <summary>
    /// 已完成
    /// </summary>
    [Display(Name = "Completed")]
    Completed,
    
    /// <summary>
    /// 已取消
    /// </summary>
    [Display(Name = "Cancelled")]
    Cancelled
}
