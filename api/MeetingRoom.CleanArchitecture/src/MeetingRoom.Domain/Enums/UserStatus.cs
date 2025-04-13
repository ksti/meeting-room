using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Domain.Enums;

/// <summary>
/// 用户状态枚举
/// </summary>
public enum UserStatus
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
    Disabled,
    
    /// <summary>
    /// 已锁定
    /// </summary>
    [Display(Name = "Locked")]
    Locked
}
