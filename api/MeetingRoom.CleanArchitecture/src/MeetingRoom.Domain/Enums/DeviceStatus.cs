using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Domain.Enums;

/// <summary>
/// 设备状态枚举
/// </summary>
public enum DeviceStatus
{
    /// <summary>
    /// 活跃状态
    /// </summary>
    [Display(Name = "Active")]
    Active,
    
    /// <summary>
    /// 已禁用
    /// </summary>
    [Display(Name = "Disabled")]
    Disabled
}
