namespace MeetingRoom.Domain.Shared;

/// <summary>
/// 可审计实体接口
/// 定义了实体的审计属性，如创建时间、更新时间等
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// 创建人
    /// </summary>
    string? CreatedBy { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// 更新人
    /// </summary>
    string? UpdatedBy { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    bool IsDeleted { get; set; }
    
    /// <summary>
    /// 删除人
    /// </summary>
    string? DeletedBy { get; set; }
    
    /// <summary>
    /// 删除时间
    /// </summary>
    DateTime? DeletedAt { get; set; }
}
