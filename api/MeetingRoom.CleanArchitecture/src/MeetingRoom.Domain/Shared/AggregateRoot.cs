using MeetingRoom.Domain.Events;

namespace MeetingRoom.Domain.Shared;

/// <summary>
/// 聚合根基类
/// 聚合根是一个实体，作为聚合的入口点，负责维护聚合内部的一致性
/// 聚合根是外部访问聚合内部对象的唯一入口
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = new();
    
    /// <summary>
    /// 获取领域事件（只读）
    /// </summary>
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected AggregateRoot() : base()
    {
    }
    
    protected AggregateRoot(string id) : base(id)
    {
    }
    
    /// <summary>
    /// 添加领域事件
    /// </summary>
    /// <param name="domainEvent">领域事件</param>
    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    /// <summary>
    /// 清除领域事件
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
