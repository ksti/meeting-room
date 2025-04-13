namespace MeetingRoom.Domain.Shared;

/// <summary>
/// 可审计实体基类
/// 实现了IAuditableEntity接口，提供了审计属性的基本实现
/// </summary>
public abstract class AuditableEntity : Entity, IAuditableEntity
{
    /// <summary>
    /// 创建人
    /// </summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 更新人
    /// </summary>
    public string? UpdatedBy { get; set; }
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// 是否已删除（软删除标记）
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// 删除人
    /// </summary>
    public string? DeletedBy { get; set; }
    
    /// <summary>
    /// 删除时间
    /// </summary>
    public DateTime? DeletedAt { get; set; }
    
    protected AuditableEntity() : base()
    {
    }
    
    protected AuditableEntity(string id) : base(id)
    {
    }
    
    /// <summary>
    /// 设置创建信息
    /// </summary>
    /// <param name="createdBy">创建人</param>
    public virtual void SetCreated(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        SetModified(createdBy);
    }
    
    /// <summary>
    /// 设置修改信息
    /// </summary>
    /// <param name="modifiedBy">修改人</param>
    public virtual void SetModified(string modifiedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = modifiedBy;
    }
    
    /// <summary>
    /// 软删除
    /// </summary>
    /// <param name="deletedBy">删除人</param>
    public virtual void SoftDelete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        SetModified(deletedBy);
    }
}
