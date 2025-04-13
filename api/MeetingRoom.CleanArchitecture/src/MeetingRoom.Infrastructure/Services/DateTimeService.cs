using MeetingRoom.Application.Interfaces;

namespace MeetingRoom.Infrastructure.Services;

/// <summary>
/// 日期时间服务实现
/// 用于获取当前日期时间，便于测试和时区处理
/// </summary>
public class DateTimeService : IDateTime
{
    /// <summary>
    /// 获取当前UTC时间
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
    
    /// <summary>
    /// 获取当前本地时间
    /// </summary>
    public DateTime Now => DateTime.Now;
}
