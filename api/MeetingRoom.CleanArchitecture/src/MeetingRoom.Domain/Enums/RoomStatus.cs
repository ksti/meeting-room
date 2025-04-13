using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Domain.Enums;

/// <summary>
/// 会议室状态枚举
/// </summary>
public enum RoomStatus
{
    /// <summary>
    /// 空闲状态
    /// </summary>
    [Display(Name = "Idle")]
    Idle,
    
    /// <summary>
    /// 占用中
    /// </summary>
    [Display(Name = "Occupied")]
    Occupied,
    
    /// <summary>
    /// 维护中
    /// </summary>
    [Display(Name = "Maintenance")]
    Maintenance,
    
    /// <summary>
    /// 已禁用
    /// </summary>
    [Display(Name = "Disabled")]
    Disabled
}
