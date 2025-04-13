namespace MeetingRoom.Application.DTOs;

/// <summary>
/// 用户数据传输对象
/// 用于在应用层和表示层之间传输用户数据
/// </summary>
public class UserDto
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
    /// 电子邮件
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// 姓
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// 名
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// 全名
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// 联系方式
    /// </summary>
    public string? Contact { get; set; }
    
    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }
    
    /// <summary>
    /// 角色
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// 状态
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
}
