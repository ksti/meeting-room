namespace MeetingRoom.Application.Common;

/// <summary>
/// 分页列表类
/// 用于封装分页查询的结果
/// </summary>
/// <typeparam name="T">列表项类型</typeparam>
public class PaginatedList<T>
{
    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageNumber { get; }
    
    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; }
    
    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages { get; }
    
    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; }
    
    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
    
    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
    
    /// <summary>
    /// 列表项
    /// </summary>
    public List<T> Items { get; }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="items">列表项</param>
    /// <param name="totalCount">总记录数</param>
    /// <param name="pageNumber">当前页码</param>
    /// <param name="pageSize">每页大小</param>
    public PaginatedList(List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        Items = items;
    }
    
    /// <summary>
    /// 创建分页列表
    /// </summary>
    /// <param name="source">源数据</param>
    /// <param name="pageNumber">当前页码</param>
    /// <param name="pageSize">每页大小</param>
    /// <returns>分页列表</returns>
    public static PaginatedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}
