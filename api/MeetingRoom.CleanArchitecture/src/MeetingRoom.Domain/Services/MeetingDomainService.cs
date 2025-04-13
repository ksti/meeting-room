using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Interfaces.Repositories;
using MeetingRoom.Domain.Interfaces.Services;

namespace MeetingRoom.Domain.Services;

/// <summary>
/// 会议领域服务
/// 实现会议相关的领域逻辑，特别是涉及多个聚合的操作
/// </summary>
public class MeetingDomainService : IMeetingDomainService
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IRoomRepository _roomRepository;
    
    public MeetingDomainService(IMeetingRepository meetingRepository, IRoomRepository roomRepository)
    {
        _meetingRepository = meetingRepository ?? throw new ArgumentNullException(nameof(meetingRepository));
        _roomRepository = roomRepository ?? throw new ArgumentNullException(nameof(roomRepository));
    }
    
    /// <summary>
    /// 检查会议时间是否可用
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID（用于更新会议时）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否可用</returns>
    public async Task<bool> IsMeetingTimeAvailableAsync(
        string roomId,
        DateTime startTime,
        DateTime endTime,
        string? excludeMeetingId = null,
        CancellationToken cancellationToken = default)
    {
        // 检查会议室是否存在且可用
        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room == null || !room.IsAvailable())
        {
            return false;
        }
        
        // 检查是否有时间冲突的会议
        return !await _meetingRepository.HasConflictingMeetingsAsync(
            roomId,
            startTime,
            endTime,
            excludeMeetingId,
            cancellationToken);
    }
    
    /// <summary>
    /// 获取指定时间范围内可用的会议室列表
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="requiredCapacity">所需容量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>可用会议室ID列表</returns>
    public async Task<List<string>> GetAvailableRoomsForTimeRangeAsync(
        DateTime startTime,
        DateTime endTime,
        int requiredCapacity = 0,
        CancellationToken cancellationToken = default)
    {
        // 获取所有可用的会议室
        var availableRooms = await _roomRepository.GetAvailableRoomsAsync(requiredCapacity, cancellationToken);
        
        if (!availableRooms.Any())
        {
            return new List<string>();
        }
        
        // 获取该时间段内的所有会议
        var meetings = await _meetingRepository.GetMeetingsByTimeRangeAsync(startTime, endTime, cancellationToken);
        
        // 找出有冲突的会议室ID
        var conflictingRoomIds = meetings
            .Where(m => m.Status != MeetingStatus.Cancelled) // 排除已取消的会议
            .Select(m => m.RoomId)
            .Distinct()
            .ToHashSet();
        
        // 返回没有冲突的会议室ID
        return availableRooms
            .Where(r => !conflictingRoomIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToList();
    }
    
    /// <summary>
    /// 自动更新会议状态
    /// 例如：将已过期的会议标记为已完成
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新的会议数量</returns>
    public async Task<int> UpdateMeetingStatusesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var updateCount = 0;
        
        // 获取所有已安排的会议
        var scheduledMeetings = await _meetingRepository.GetMeetingsByStatusAsync(MeetingStatus.Scheduled, cancellationToken);
        
        // 将当前时间在会议时间范围内的会议标记为进行中
        foreach (var meeting in scheduledMeetings.Where(m => m.TimeRange.IsTimeInRange(now)))
        {
            meeting.Start();
            await _meetingRepository.UpdateAsync(meeting, cancellationToken);
            updateCount++;
        }
        
        // 获取所有进行中的会议
        var inProgressMeetings = await _meetingRepository.GetMeetingsByStatusAsync(MeetingStatus.InProgress, cancellationToken);
        
        // 将已结束的会议标记为已完成
        foreach (var meeting in inProgressMeetings.Where(m => now > m.TimeRange.End))
        {
            meeting.Complete();
            await _meetingRepository.UpdateAsync(meeting, cancellationToken);
            updateCount++;
        }
        
        return updateCount;
    }
    
    /// <summary>
    /// 检查用户是否有权限操作会议
    /// </summary>
    /// <param name="meeting">会议</param>
    /// <param name="userId">用户ID</param>
    /// <param name="requireOrganizer">是否要求用户是组织者</param>
    /// <returns>是否有权限</returns>
    public bool CanUserManageMeeting(Meeting meeting, string userId, bool requireOrganizer = false)
    {
        if (meeting == null || string.IsNullOrEmpty(userId))
        {
            return false;
        }
        
        // 如果要求用户是组织者，则只有组织者有权限
        if (requireOrganizer)
        {
            return meeting.IsOrganizer(userId);
        }
        
        // 否则，组织者和参与者都有权限
        return meeting.IsOrganizer(userId) || meeting.IsParticipant(userId);
    }
}
