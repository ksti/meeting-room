namespace MeetingRoom.Domain.Events;

/// <summary>
/// 领域事件基类
/// 领域事件用于表示领域中发生的重要事件，可以用于解耦领域模型中的不同部分
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// 事件ID
    /// </summary>
    public string Id { get; }
    
    /// <summary>
    /// 事件发生时间
    /// </summary>
    public DateTime OccurredOn { get; }
    
    protected DomainEvent()
    {
        Id = Guid.NewGuid().ToString();
        OccurredOn = DateTime.UtcNow;
    }
}
