using System.ComponentModel.DataAnnotations;

namespace MeetingRoom.Domain.Enums;

/// <summary>
/// 用户角色枚举
/// </summary>
public enum UserRole
{
    /// <summary>
    /// 管理员
    /// </summary>
    [Display(Name = "Admin")]
    Admin,
    
    /// <summary>
    /// 普通用户
    /// </summary>
    [Display(Name = "User")]
    User
}
