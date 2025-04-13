namespace MeetingRoom.Application.Interfaces;

/// <summary>
/// 日期时间服务接口
/// 用于获取当前日期时间，便于测试和时区处理
/// </summary>
public interface IDateTime
{
    /// <summary>
    /// 获取当前UTC时间
    /// </summary>
    DateTime UtcNow { get; }
    
    /// <summary>
    /// 获取当前本地时间
    /// </summary>
    DateTime Now { get; }
}
