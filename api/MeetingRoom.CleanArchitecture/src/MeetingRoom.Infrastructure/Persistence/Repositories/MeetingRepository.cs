using Microsoft.EntityFrameworkCore;
using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Infrastructure.Persistence.Repositories;

/// <summary>
/// 会议仓储实现
/// 实现IMeetingRepository接口，提供会议相关的数据访问功能
/// </summary>
public class MeetingRepository : RepositoryBase<Meeting>, IMeetingRepository
{
    public MeetingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    /// <summary>
    /// 获取会议集合
    /// </summary>
    protected override IQueryable<Meeting> EntitySet => _dbContext.Meetings;
    
    /// <summary>
    /// 根据ID获取会议，包括参与者
    /// </summary>
    /// <param name="id">会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议，如果不存在则返回null</returns>
    public async Task<Meeting?> GetByIdWithParticipantsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }
    
    /// <summary>
    /// 获取用户的会议列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    public async Task<List<Meeting>> GetUserMeetingsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = EntitySet
            .Where(m => 
                m.OrganizerId == userId || // 用户是组织者
                m.ParticipantIds.Contains(userId)); // 用户是参与者
        
        // 筛选日期范围
        if (startDate.HasValue)
        {
            var start = startDate.Value.Date;
            query = query.Where(m => m.TimeRange.End >= start);
        }
        
        if (endDate.HasValue)
        {
            var end = endDate.Value.Date.AddDays(1); // 包含结束日期当天
            query = query.Where(m => m.TimeRange.Start < end);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取会议室的会议列表
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    public async Task<List<Meeting>> GetRoomMeetingsAsync(string roomId, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = EntitySet
            .Where(m => m.RoomId == roomId);
        
        // 筛选日期范围
        if (startDate.HasValue)
        {
            var start = startDate.Value.Date;
            query = query.Where(m => m.TimeRange.End >= start);
        }
        
        if (endDate.HasValue)
        {
            var end = endDate.Value.Date.AddDays(1); // 包含结束日期当天
            query = query.Where(m => m.TimeRange.Start < end);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 检查会议室在指定时间段内是否可用
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否可用</returns>
    public async Task<bool> IsRoomAvailableAsync(string roomId, DateTime startTime, DateTime endTime, string? excludeMeetingId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Meetings
            .Where(m => 
                m.RoomId == roomId &&
                m.Status != Domain.Enums.MeetingStatus.Cancelled && // 未取消的会议
                (m.TimeRange.Start < endTime && m.TimeRange.End > startTime)); // 时间段重叠
        
        // 排除指定会议
        if (!string.IsNullOrEmpty(excludeMeetingId))
        {
            query = query.Where(m => m.Id != excludeMeetingId);
        }
        
        // 如果查询结果为空，则会议室可用
        return !await query.AnyAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取即将开始的会议
    /// </summary>
    /// <param name="minutesThreshold">分钟阈值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>即将开始的会议列表</returns>
    public async Task<List<Meeting>> GetUpcomingMeetingsAsync(int minutesThreshold, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddMinutes(minutesThreshold);
        
        return await EntitySet
            .Where(m => 
                m.Status == Domain.Enums.MeetingStatus.Scheduled && // 已安排的会议
                m.TimeRange.Start > now && // 未开始
                m.TimeRange.Start <= threshold) // 在阈值内
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取正在进行的会议
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>正在进行的会议列表</returns>
    public async Task<List<Meeting>> GetOngoingMeetingsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        return await EntitySet
            .Where(m => 
                m.Status == Domain.Enums.MeetingStatus.InProgress || // 进行中的会议
                (m.Status == Domain.Enums.MeetingStatus.Scheduled && // 已安排但应该已经开始的会议
                 m.TimeRange.Start <= now && 
                 m.TimeRange.End > now))
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取指定会议室在特定时间范围内的会议列表
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    public async Task<List<Meeting>> GetMeetingsByRoomAndTimeRangeAsync(
        string roomId,
        DateTime startTime,
        DateTime endTime,
        string? excludeMeetingId = null,
        CancellationToken cancellationToken = default)
    {
        var query = EntitySet
            .Where(m => 
                m.RoomId == roomId &&
                m.Status != Domain.Enums.MeetingStatus.Cancelled && // 未取消的会议
                (m.TimeRange.Start < endTime && m.TimeRange.End > startTime)); // 时间段重叠
        
        // 排除指定会议
        if (!string.IsNullOrEmpty(excludeMeetingId))
        {
            query = query.Where(m => m.Id != excludeMeetingId);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取用户组织的会议列表
    /// </summary>
    /// <param name="organizerId">组织者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    public async Task<List<Meeting>> GetMeetingsByOrganizerAsync(string organizerId, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .Where(m => m.OrganizerId == organizerId)
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取用户参与的会议列表
    /// </summary>
    /// <param name="attendeeId">参与者ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    public async Task<List<Meeting>> GetMeetingsByAttendeeAsync(string attendeeId, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .Where(m => m.ParticipantIds.Contains(attendeeId))
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取指定状态的会议列表
    /// </summary>
    /// <param name="status">会议状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    public async Task<List<Meeting>> GetMeetingsByStatusAsync(Domain.Enums.MeetingStatus status, CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .Where(m => m.Status == status)
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取指定时间范围内的会议列表
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议列表</returns>
    public async Task<List<Meeting>> GetMeetingsByTimeRangeAsync(
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default)
    {
        return await EntitySet
            .Where(m => 
                m.Status != Domain.Enums.MeetingStatus.Cancelled && // 未取消的会议
                (m.TimeRange.Start < endTime && m.TimeRange.End > startTime)) // 时间段重叠
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 检查会议室在指定时间范围内是否有冲突的会议
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="excludeMeetingId">排除的会议ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否有冲突</returns>
    public async Task<bool> HasConflictingMeetingsAsync(
        string roomId,
        DateTime startTime,
        DateTime endTime,
        string? excludeMeetingId = null,
        CancellationToken cancellationToken = default)
    {
        // 这个方法与 IsRoomAvailableAsync 的逻辑相反
        return !await IsRoomAvailableAsync(roomId, startTime, endTime, excludeMeetingId, cancellationToken);
    }
}
