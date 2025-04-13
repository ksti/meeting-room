using MeetingRoom.Domain.Aggregates.RoomAggregate;
using MeetingRoom.Domain.Enums;

namespace MeetingRoom.Domain.Interfaces.Repositories;

/// <summary>
/// 会议室仓储接口
/// 定义了会议室聚合根特有的仓储操作
/// </summary>
public interface IRoomRepository : IRepository<Room>
{
    /// <summary>
    /// 根据会议室名称获取会议室
    /// </summary>
    /// <param name="name">会议室名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议室，如果不存在则返回null</returns>
    Task<Room?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查会议室名称是否已存在
    /// </summary>
    /// <param name="name">会议室名称</param>
    /// <param name="excludeRoomId">排除的会议室ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsNameExistsAsync(string name, string? excludeRoomId = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取指定状态的会议室列表
    /// </summary>
    /// <param name="status">会议室状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议室列表</returns>
    Task<List<Room>> GetRoomsByStatusAsync(RoomStatus status, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取可用的会议室列表
    /// </summary>
    /// <param name="capacity">最小容量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>会议室列表</returns>
    Task<List<Room>> GetAvailableRoomsAsync(int capacity = 0, CancellationToken cancellationToken = default);
}
