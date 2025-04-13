using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MeetingRoom.Domain.Interfaces.Repositories;
using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Infrastructure.Persistence.Repositories;

/// <summary>
/// 仓储基类
/// 实现IRepository接口，提供通用的CRUD操作
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public abstract class RepositoryBase<T> : IRepository<T> where T : Entity
{
    protected readonly ApplicationDbContext _dbContext;
    
    protected RepositoryBase(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// 获取实体集合
    /// </summary>
    protected virtual IQueryable<T> EntitySet => _dbContext.Set<T>();
    
    /// <summary>
    /// 根据ID获取实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体，如果不存在则返回null</returns>
    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await EntitySet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    
    /// <summary>
    /// 根据条件获取单个实体
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体，如果不存在则返回null</returns>
    public virtual async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await EntitySet.FirstOrDefaultAsync(predicate, cancellationToken);
    }
    
    /// <summary>
    /// 根据条件获取实体列表
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体列表</returns>
    public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = EntitySet;
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 根据ID列表获取实体列表
    /// </summary>
    /// <param name="ids">ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体列表</returns>
    public virtual async Task<List<T>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        return await EntitySet.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// 添加实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// 根据ID删除实体
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public virtual async Task DeleteByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        
        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }
    
    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="id">实体ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    public virtual async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        return await EntitySet.AnyAsync(e => e.Id == id, cancellationToken);
    }
    
    /// <summary>
    /// 检查实体是否存在
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await EntitySet.AnyAsync(predicate, cancellationToken);
    }
    
    /// <summary>
    /// 获取实体数量
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>实体数量</returns>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = EntitySet;
        
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        return await query.CountAsync(cancellationToken);
    }
}
