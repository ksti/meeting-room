namespace MeetingRoom.Application.Common.Exceptions;

/// <summary>
/// 资源未找到异常
/// 当请求的资源不存在时抛出
/// </summary>
public class NotFoundException : ApplicationException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="key">资源标识</param>
    public NotFoundException(string name, object key)
        : base($"实体 \"{name}\" ({key}) 未找到。")
    {
        Name = name;
        Key = key;
    }
    
    /// <summary>
    /// 资源名称
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// 资源标识
    /// </summary>
    public object Key { get; }
}
