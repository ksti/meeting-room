using MeetingRoom.Domain.Aggregates.MeetingAggregate;

namespace MeetingRoom.Domain.Interfaces.Services;

/// <summary>
/// 会议领域服务接口
/// 处理不适合放在会议聚合根中的领域逻辑
/// </summary>
public interface IMeetingDomainService
{
    /// <summary>
    /// 检查会议时间是否可用
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID（用于更新会议时）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否可用</returns>
    Task<bool> IsMeetingTimeAvailableAsync(
        string roomId,
        DateTime startTime,
        DateTime endTime,
        string? excludeMeetingId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取指定时间范围内可用的会议室列表
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="requiredCapacity">所需容量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>可用会议室ID列表</returns>
    Task<List<string>> GetAvailableRoomsForTimeRangeAsync(
        DateTime startTime,
        DateTime endTime,
        int requiredCapacity = 0,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 自动更新会议状态
    /// 例如：将已过期的会议标记为已完成
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新的会议数量</returns>
    Task<int> UpdateMeetingStatusesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查用户是否有权限操作会议
    /// </summary>
    /// <param name="meeting">会议</param>
    /// <param name="userId">用户ID</param>
    /// <param name="requireOrganizer">是否要求用户是组织者</param>
    /// <returns>是否有权限</returns>
    bool CanUserManageMeeting(Meeting meeting, string userId, bool requireOrganizer = false);
}
