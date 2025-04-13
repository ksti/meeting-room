using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Domain.ValueObjects;

/// <summary>
/// 时间范围值对象
/// 表示一个时间段，包含开始时间和结束时间
/// 作为值对象，TimeRange是不可变的，一旦创建就不能修改
/// </summary>
public class TimeRange : ValueObject
{
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime Start { get; }
    
    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime End { get; }
    
    /// <summary>
    /// 时长（分钟）
    /// </summary>
    public int DurationMinutes => (int)(End - Start).TotalMinutes;
    
    private TimeRange(DateTime start, DateTime end)
    {
        if (start >= end)
        {
            throw new TimeRangeException("结束时间必须晚于开始时间");
        }
        
        Start = start;
        End = end;
    }
    
    /// <summary>
    /// 创建时间范围
    /// </summary>
    /// <param name="start">开始时间</param>
    /// <param name="end">结束时间</param>
    /// <returns>时间范围值对象</returns>
    public static TimeRange Create(DateTime start, DateTime end)
    {
        return new TimeRange(start, end);
    }
    
    /// <summary>
    /// 检查是否与另一个时间范围重叠
    /// </summary>
    /// <param name="other">另一个时间范围</param>
    /// <returns>是否重叠</returns>
    public bool Overlaps(TimeRange other)
    {
        return Start < other.End && other.Start < End;
    }
    
    /// <summary>
    /// 检查当前时间是否在时间范围内
    /// </summary>
    /// <returns>是否在范围内</returns>
    public bool IsCurrentTimeInRange()
    {
        var now = DateTime.UtcNow;
        return now >= Start && now <= End;
    }
    
    /// <summary>
    /// 检查指定时间是否在时间范围内
    /// </summary>
    /// <param name="time">指定时间</param>
    /// <returns>是否在范围内</returns>
    public bool IsTimeInRange(DateTime time)
    {
        return time >= Start && time <= End;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }
    
    public override string ToString()
    {
        return $"{Start:yyyy-MM-dd HH:mm} - {End:yyyy-MM-dd HH:mm}";
    }
}

/// <summary>
/// 时间范围相关的领域异常
/// </summary>
public class TimeRangeException : DomainException
{
    public TimeRangeException(string message) : base(message)
    {
    }
}
