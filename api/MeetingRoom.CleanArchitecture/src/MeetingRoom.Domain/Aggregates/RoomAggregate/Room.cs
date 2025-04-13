using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Events;
using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Domain.Aggregates.RoomAggregate;

/// <summary>
/// 会议室聚合根
/// 会议室是一个聚合根，包含会议室的基本信息和相关操作
/// </summary>
public class Room : AuditableAggregateRoot
{
    /// <summary>
    /// 会议室名称
    /// </summary>
    public string Name { get; private set; }
    
    /// <summary>
    /// 会议室描述
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// 会议室容量
    /// </summary>
    public int Capacity { get; private set; }
    
    /// <summary>
    /// 会议室状态
    /// </summary>
    public RoomStatus Status { get; private set; }
    
    // 私有构造函数，防止直接创建实例
    private Room() 
    { 
        Name = string.Empty;
        Description = string.Empty;
        Status = RoomStatus.Idle;
    }
    
    private Room(string name, string description, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new RoomDomainException("会议室名称不能为空");
        }
        
        if (capacity <= 0)
        {
            throw new RoomDomainException("会议室容量必须大于0");
        }
        
        Name = name;
        Description = description ?? string.Empty;
        Capacity = capacity;
        Status = RoomStatus.Idle;
        
        // 添加会议室创建事件
        AddDomainEvent(new RoomCreatedEvent(Id, name));
    }
    
    /// <summary>
    /// 创建新会议室
    /// </summary>
    /// <param name="name">会议室名称</param>
    /// <param name="description">会议室描述</param>
    /// <param name="capacity">会议室容量</param>
    /// <returns>会议室聚合根</returns>
    public static Room Create(string name, string description, int capacity)
    {
        return new Room(name, description, capacity);
    }
    
    /// <summary>
    /// 更新会议室信息
    /// </summary>
    /// <param name="name">会议室名称</param>
    /// <param name="description">会议室描述</param>
    /// <param name="capacity">会议室容量</param>
    public void Update(string name, string description, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new RoomDomainException("会议室名称不能为空");
        }
        
        if (capacity <= 0)
        {
            throw new RoomDomainException("会议室容量必须大于0");
        }
        
        Name = name;
        Description = description ?? string.Empty;
        Capacity = capacity;
        
        AddDomainEvent(new RoomUpdatedEvent(Id, name));
    }
    
    /// <summary>
    /// 设置会议室状态
    /// </summary>
    /// <param name="status">新状态</param>
    public void SetStatus(RoomStatus status)
    {
        if (Status == status)
        {
            return;
        }
        
        Status = status;
        
        AddDomainEvent(new RoomStatusChangedEvent(Id, status));
    }
    
    /// <summary>
    /// 设置会议室为空闲状态
    /// </summary>
    public void SetIdle()
    {
        SetStatus(RoomStatus.Idle);
    }
    
    /// <summary>
    /// 设置会议室为占用状态
    /// </summary>
    public void SetOccupied()
    {
        SetStatus(RoomStatus.Occupied);
    }
    
    /// <summary>
    /// 设置会议室为维护状态
    /// </summary>
    public void SetMaintenance()
    {
        SetStatus(RoomStatus.Maintenance);
    }
    
    /// <summary>
    /// 设置会议室为禁用状态
    /// </summary>
    public void SetDisabled()
    {
        SetStatus(RoomStatus.Disabled);
    }
    
    /// <summary>
    /// 检查会议室是否可用
    /// </summary>
    /// <returns>是否可用</returns>
    public bool IsAvailable()
    {
        return Status == RoomStatus.Idle;
    }
    
    /// <summary>
    /// 检查会议室是否有足够容量
    /// </summary>
    /// <param name="requiredCapacity">所需容量</param>
    /// <returns>是否有足够容量</returns>
    public bool HasSufficientCapacity(int requiredCapacity)
    {
        return Capacity >= requiredCapacity;
    }
}

/// <summary>
/// 会议室相关的领域异常
/// </summary>
public class RoomDomainException : DomainException
{
    public RoomDomainException(string message) : base(message)
    {
    }
}

/// <summary>
/// 会议室创建事件
/// </summary>
public class RoomCreatedEvent : DomainEvent
{
    public string RoomId { get; }
    public string RoomName { get; }
    
    public RoomCreatedEvent(string roomId, string roomName)
    {
        RoomId = roomId;
        RoomName = roomName;
    }
}

/// <summary>
/// 会议室更新事件
/// </summary>
public class RoomUpdatedEvent : DomainEvent
{
    public string RoomId { get; }
    public string RoomName { get; }
    
    public RoomUpdatedEvent(string roomId, string roomName)
    {
        RoomId = roomId;
        RoomName = roomName;
    }
}

/// <summary>
/// 会议室状态变更事件
/// </summary>
public class RoomStatusChangedEvent : DomainEvent
{
    public string RoomId { get; }
    public RoomStatus NewStatus { get; }
    
    public RoomStatusChangedEvent(string roomId, RoomStatus newStatus)
    {
        RoomId = roomId;
        NewStatus = newStatus;
    }
}
