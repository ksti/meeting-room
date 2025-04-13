using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Events;
using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;
using MeetingRoom.Domain.ValueObjects;

namespace MeetingRoom.Domain.Aggregates.MeetingAggregate;

/// <summary>
/// 会议聚合根
/// 会议是一个聚合根，包含会议的基本信息、参与者和相关操作
/// </summary>
public class Meeting : AuditableAggregateRoot
{
    /// <summary>
    /// 会议标题
    /// </summary>
    public string Title { get; private set; }
    
    /// <summary>
    /// 会议描述
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// 会议容量
    /// </summary>
    public int Capacity { get; private set; }
    
    /// <summary>
    /// 会议时间范围
    /// </summary>
    public TimeRange TimeRange { get; private set; }
    
    /// <summary>
    /// 组织者ID
    /// </summary>
    public string OrganizerId { get; private set; }
    
    /// <summary>
    /// 会议室ID
    /// </summary>
    public string RoomId { get; private set; }
    
    /// <summary>
    /// 会议状态
    /// </summary>
    public MeetingStatus Status { get; private set; }
    
    /// <summary>
    /// 参与者ID集合
    /// </summary>
    private readonly List<string> _participantIds = new();
    
    /// <summary>
    /// 参与者ID集合（只读）
    /// </summary>
    public IReadOnlyCollection<string> ParticipantIds => _participantIds.AsReadOnly();
    
    // 私有构造函数，防止直接创建实例
    private Meeting() 
    { 
        Title = string.Empty;
        Capacity = 0;
        TimeRange = TimeRange.Create(DateTime.MinValue, DateTime.MinValue.AddHours(1));
        OrganizerId = string.Empty;
        RoomId = string.Empty;
        Status = MeetingStatus.Cancelled;
    }
    
    private Meeting(
        string title,
        string? description,
        int capacity,
        TimeRange timeRange,
        string organizerId,
        string roomId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new MeetingDomainException("会议标题不能为空");
        }
        
        if (capacity <= 0)
        {
            throw new MeetingDomainException("会议容量必须大于0");
        }
        
        Title = title;
        Description = description;
        Capacity = capacity;
        TimeRange = timeRange;
        OrganizerId = organizerId;
        RoomId = roomId;
        Status = MeetingStatus.Scheduled;
        
        // 添加组织者作为参与者
        _participantIds.Add(organizerId);
        
        // 添加会议创建事件
        AddDomainEvent(new MeetingCreatedEvent(Id, title, timeRange.Start, timeRange.End, roomId, organizerId));
    }
    
    /// <summary>
    /// 创建新会议
    /// </summary>
    /// <param name="title">会议标题</param>
    /// <param name="description">会议描述</param>
    /// <param name="capacity">会议容量</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="organizerId">组织者ID</param>
    /// <param name="roomId">会议室ID</param>
    /// <returns>会议聚合根</returns>
    public static Meeting Create(
        string title,
        string? description,
        int capacity,
        DateTime startTime,
        DateTime endTime,
        string organizerId,
        string roomId)
    {
        var timeRange = TimeRange.Create(startTime, endTime);
        
        return new Meeting(title, description, capacity, timeRange, organizerId, roomId);
    }
    
    /// <summary>
    /// 更新会议信息
    /// </summary>
    /// <param name="title">会议标题</param>
    /// <param name="description">会议描述</param>
    /// <param name="capacity">会议容量</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    public void Update(
        string title,
        string? description,
        int capacity,
        DateTime startTime,
        DateTime endTime)
    {
        if (Status == MeetingStatus.Cancelled || Status == MeetingStatus.Completed)
        {
            throw new MeetingDomainException("已取消或已完成的会议不能更新");
        }
        
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new MeetingDomainException("会议标题不能为空");
        }
        
        if (capacity < _participantIds.Count)
        {
            throw new MeetingDomainException($"会议容量不能小于当前参与者数量 {_participantIds.Count}");
        }
        
        var newTimeRange = TimeRange.Create(startTime, endTime);
        
        Title = title;
        Description = description;
        Capacity = capacity;
        TimeRange = newTimeRange;
        
        AddDomainEvent(new MeetingUpdatedEvent(Id, title, startTime, endTime));
    }
    
    /// <summary>
    /// 添加参与者
    /// </summary>
    /// <param name="participantId">参与者ID</param>
    public void AddParticipant(string participantId)
    {
        if (Status == MeetingStatus.Cancelled || Status == MeetingStatus.Completed)
        {
            throw new MeetingDomainException("已取消或已完成的会议不能添加参与者");
        }
        
        if (_participantIds.Contains(participantId))
        {
            return; // 已经是参与者，无需重复添加
        }
        
        if (_participantIds.Count >= Capacity)
        {
            throw new MeetingDomainException("会议已达到最大容量");
        }
        
        _participantIds.Add(participantId);
        
        AddDomainEvent(new MeetingParticipantAddedEvent(Id, participantId));
    }
    
    /// <summary>
    /// 移除参与者
    /// </summary>
    /// <param name="participantId">参与者ID</param>
    public void RemoveParticipant(string participantId)
    {
        if (Status == MeetingStatus.Cancelled || Status == MeetingStatus.Completed)
        {
            throw new MeetingDomainException("已取消或已完成的会议不能移除参与者");
        }
        
        if (participantId == OrganizerId)
        {
            throw new MeetingDomainException("不能移除会议组织者");
        }
        
        if (_participantIds.Remove(participantId))
        {
            AddDomainEvent(new MeetingParticipantRemovedEvent(Id, participantId));
        }
    }
    
    /// <summary>
    /// 取消会议
    /// </summary>
    public void Cancel()
    {
        if (Status == MeetingStatus.Cancelled)
        {
            return;
        }
        
        if (Status == MeetingStatus.Completed)
        {
            throw new MeetingDomainException("已完成的会议不能取消");
        }
        
        Status = MeetingStatus.Cancelled;
        
        AddDomainEvent(new MeetingCancelledEvent(Id));
    }
    
    /// <summary>
    /// 开始会议
    /// </summary>
    public void Start()
    {
        if (Status != MeetingStatus.Scheduled)
        {
            throw new MeetingDomainException("只有已安排的会议才能开始");
        }
        
        Status = MeetingStatus.InProgress;
        
        AddDomainEvent(new MeetingStartedEvent(Id));
    }
    
    /// <summary>
    /// 完成会议
    /// </summary>
    public void Complete()
    {
        if (Status != MeetingStatus.InProgress)
        {
            throw new MeetingDomainException("只有进行中的会议才能完成");
        }
        
        Status = MeetingStatus.Completed;
        
        AddDomainEvent(new MeetingCompletedEvent(Id));
    }
    
    /// <summary>
    /// 检查会议是否与指定时间范围重叠
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>是否重叠</returns>
    public bool OverlapsWith(DateTime startTime, DateTime endTime)
    {
        if (Status == MeetingStatus.Cancelled)
        {
            return false; // 已取消的会议不会导致冲突
        }
        
        var otherTimeRange = TimeRange.Create(startTime, endTime);
        return TimeRange.Overlaps(otherTimeRange);
    }
    
    /// <summary>
    /// 检查用户是否是会议参与者
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否是参与者</returns>
    public bool IsParticipant(string userId)
    {
        return _participantIds.Contains(userId);
    }
    
    /// <summary>
    /// 检查用户是否是会议组织者
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否是组织者</returns>
    public bool IsOrganizer(string userId)
    {
        return OrganizerId == userId;
    }
    
    /// <summary>
    /// 检查会议是否已满
    /// </summary>
    /// <returns>是否已满</returns>
    public bool IsFull()
    {
        return _participantIds.Count >= Capacity;
    }
    
    /// <summary>
    /// 获取剩余容量
    /// </summary>
    /// <returns>剩余容量</returns>
    public int GetRemainingCapacity()
    {
        return Capacity - _participantIds.Count;
    }
}

/// <summary>
/// 会议相关的领域异常
/// </summary>
public class MeetingDomainException : DomainException
{
    public MeetingDomainException(string message) : base(message)
    {
    }
}

/// <summary>
/// 会议创建事件
/// </summary>
public class MeetingCreatedEvent : DomainEvent
{
    public string MeetingId { get; }
    public string Title { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public string RoomId { get; }
    public string OrganizerId { get; }
    
    public MeetingCreatedEvent(string meetingId, string title, DateTime startTime, DateTime endTime, string roomId, string organizerId)
    {
        MeetingId = meetingId;
        Title = title;
        StartTime = startTime;
        EndTime = endTime;
        RoomId = roomId;
        OrganizerId = organizerId;
    }
}

/// <summary>
/// 会议更新事件
/// </summary>
public class MeetingUpdatedEvent : DomainEvent
{
    public string MeetingId { get; }
    public string Title { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    
    public MeetingUpdatedEvent(string meetingId, string title, DateTime startTime, DateTime endTime)
    {
        MeetingId = meetingId;
        Title = title;
        StartTime = startTime;
        EndTime = endTime;
    }
}

/// <summary>
/// 会议参与者添加事件
/// </summary>
public class MeetingParticipantAddedEvent : DomainEvent
{
    public string MeetingId { get; }
    public string ParticipantId { get; }
    
    public MeetingParticipantAddedEvent(string meetingId, string participantId)
    {
        MeetingId = meetingId;
        ParticipantId = participantId;
    }
}

/// <summary>
/// 会议参与者移除事件
/// </summary>
public class MeetingParticipantRemovedEvent : DomainEvent
{
    public string MeetingId { get; }
    public string ParticipantId { get; }
    
    public MeetingParticipantRemovedEvent(string meetingId, string participantId)
    {
        MeetingId = meetingId;
        ParticipantId = participantId;
    }
}

/// <summary>
/// 会议取消事件
/// </summary>
public class MeetingCancelledEvent : DomainEvent
{
    public string MeetingId { get; }
    
    public MeetingCancelledEvent(string meetingId)
    {
        MeetingId = meetingId;
    }
}

/// <summary>
/// 会议开始事件
/// </summary>
public class MeetingStartedEvent : DomainEvent
{
    public string MeetingId { get; }
    
    public MeetingStartedEvent(string meetingId)
    {
        MeetingId = meetingId;
    }
}

/// <summary>
/// 会议完成事件
/// </summary>
public class MeetingCompletedEvent : DomainEvent
{
    public string MeetingId { get; }
    
    public MeetingCompletedEvent(string meetingId)
    {
        MeetingId = meetingId;
    }
}
