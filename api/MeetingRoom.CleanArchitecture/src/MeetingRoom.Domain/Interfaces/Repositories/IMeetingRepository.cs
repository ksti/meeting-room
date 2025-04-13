using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Enums;

namespace MeetingRoom.Domain.Interfaces.Repositories;

/// <summary>
/// 会议仓储接口
/// 定义了会议聚合根特有的仓储操作
/// </summary>
public interface IMeetingRepository : IRepository<Meeting>
{
    /// <summary>
    /// 获取指定会议室在特定时间范围内的会议列表
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    Task<List<Meeting>> GetMeetingsByRoomAndTimeRangeAsync(
        string roomId,
        DateTime startTime,
        DateTime endTime,
        string? excludeMeetingId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取用户组织的会议列表
    /// </summary>
    /// <param name="organizerId">组织者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    Task<List<Meeting>> GetMeetingsByOrganizerAsync(string organizerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取用户参与的会议列表
    /// </summary>
    /// <param name="attendeeId">参与者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    Task<List<Meeting>> GetMeetingsByAttendeeAsync(string attendeeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取指定状态的会议列表
    /// </summary>
    /// <param name="status">会议状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    Task<List<Meeting>> GetMeetingsByStatusAsync(MeetingStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取指定时间范围内的会议列表
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    Task<List<Meeting>> GetMeetingsByTimeRangeAsync(
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查会议室在指定时间范围内是否有冲突的会议
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否有冲突</returns>
    Task<bool> HasConflictingMeetingsAsync(
        string roomId,
        DateTime startTime,
        DateTime endTime,
        string? excludeMeetingId = null,
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// 检查会议室在指定时间范围内是否可用
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议室是否可用</returns>
    Task<bool> IsRoomAvailableAsync(
        string roomId,
        DateTime startTime,
        DateTime endTime,
        string? excludeMeetingId = null,
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// 获取会议及其参与者信息
    /// </summary>
    /// <param name="id">会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议及其参与者信息</returns>
    Task<Meeting?> GetByIdWithParticipantsAsync(string id, CancellationToken cancellationToken = default);
}
