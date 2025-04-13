using System.Linq.Expressions;
using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Domain.Interfaces.Repositories;

/// <summary>
/// 通用仓储接口
/// 定义了所有仓储共有的基本操作
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : Entity
{
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体，如果不存在则返回null</returns>
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据条件获取单个实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体，如果不存在则返回null</returns>
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体列表</returns>
    Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取实体数量
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体数量</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}
