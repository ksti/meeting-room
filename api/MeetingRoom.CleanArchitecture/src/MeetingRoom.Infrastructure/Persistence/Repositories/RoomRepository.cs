using Microsoft.EntityFrameworkCore;
using MeetingRoom.Domain.Aggregates.RoomAggregate;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Infrastructure.Persistence.Repositories;

/// <summary>
/// 会议室仓储实现
/// 实现IRoomRepository接口，提供会议室相关的数据访问功能
/// </summary>
public class RoomRepository : RepositoryBase<Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    /// <summary>
    /// 检查会议室名称是否存在
    /// </summary>
    /// <param name="name">会议室名称</param>
    /// <param name="excludeRoomId">排除的会议室ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    public async Task<bool> IsNameExistsAsync(string name, string? excludeRoomId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Rooms.AsQueryable();
        
        if (!string.IsNullOrEmpty(excludeRoomId))
        {
            query = query.Where(r => r.Id != excludeRoomId);
        }
        
        return await query.AnyAsync(r => r.Name == name, cancellationToken);
    }
    
    /// <summary>
    /// 获取可用的会议室列表
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="minCapacity">最小容量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>可用的会议室列表</returns>
    public async Task<List<Room>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime, int? minCapacity = null, CancellationToken cancellationToken = default)
    {
        // 获取所有会议室
        var query = _dbContext.Rooms.AsQueryable();
        
        // 过滤容量
        if (minCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        }
        
        // 获取指定时间段内已预订的会议室ID
        var bookedRoomIds = await _dbContext.Meetings
            .Where(m => 
                (m.TimeRange.Start < endTime && m.TimeRange.End > startTime) && // 时间段重叠
                (m.Status != Domain.Enums.MeetingStatus.Cancelled)) // 未取消的会议
            .Select(m => m.RoomId)
            .Distinct()
            .ToListAsync(cancellationToken);
        
        // 排除已预订的会议室
        query = query.Where(r => !bookedRoomIds.Contains(r.Id));
        
        // 排除非空闲状态的会议室
        query = query.Where(r => r.Status == Domain.Enums.RoomStatus.Idle);
        
        return await query.ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取会议室使用统计
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议室使用统计</returns>
    public async Task<RoomUsageStatistics> GetRoomUsageStatisticsAsync(string roomId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        // 获取指定时间段内的会议
        var meetings = await _dbContext.Meetings
            .Where(m => 
                m.RoomId == roomId &&
                m.TimeRange.Start >= startDate &&
                m.TimeRange.Start < endDate.AddDays(1) && // 包含结束日期当天
                m.Status != Domain.Enums.MeetingStatus.Cancelled) // 未取消的会议
            .ToListAsync(cancellationToken);
        
        // 计算统计数据
        var totalMeetings = meetings.Count;
        var totalDurationMinutes = meetings.Sum(m => (int)(m.TimeRange.End - m.TimeRange.Start).TotalMinutes);
        var averageDurationMinutes = totalMeetings > 0 ? totalDurationMinutes / totalMeetings : 0;
        
        // 创建统计对象
        return new RoomUsageStatistics
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            TotalMeetings = totalMeetings,
            TotalDurationMinutes = totalDurationMinutes,
            AverageDurationMinutes = averageDurationMinutes
        };
    }
    
    /// <summary>
    /// 根据名称获取会议室
    /// </summary>
    /// <param name="name">会议室名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议室，如果不存在则返回null</returns>
    public async Task<Room?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rooms
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
    
    /// <summary>
    /// 根据状态获取会议室列表
    /// </summary>
    /// <param name="status">会议室状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议室列表</returns>
    public async Task<List<Room>> GetRoomsByStatusAsync(Domain.Enums.RoomStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rooms
            .Where(r => r.Status == status)
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 获取可用的会议室列表
    /// </summary>
    /// <param name="minCapacity">最小容量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>可用的会议室列表</returns>
    public async Task<List<Room>> GetAvailableRoomsAsync(int minCapacity, CancellationToken cancellationToken = default)
    {
        // 获取所有空闲状态的会议室，并按容量过滤
        return await _dbContext.Rooms
            .Where(r => r.Status == Domain.Enums.RoomStatus.Idle && r.Capacity >= minCapacity)
            .ToListAsync(cancellationToken);
    }
}
