using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.ValueObjects;

namespace MeetingRoom.Infrastructure.Persistence.Interceptors;

/// <summary>
/// TimeRange 值对象拦截器
/// 用于在实体加载和保存时处理 TimeRange 值对象
/// </summary>
public class TimeRangeInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        if (eventData.Context is null) return result;
        
        UpdateTimeRangeBeforeSave(eventData.Context);
        
        return result;
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return ValueTask.FromResult(result);
        
        UpdateTimeRangeBeforeSave(eventData.Context);
        
        return ValueTask.FromResult(result);
    }
    
    private static void UpdateTimeRangeBeforeSave(DbContext context)
    {
        // 获取所有正在被跟踪的 Meeting 实体
        var meetings = context.ChangeTracker.Entries<Meeting>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        
        foreach (var meeting in meetings)
        {
            // 将 TimeRange 值对象的值保存到影子属性中
            if (meeting.Entity.TimeRange != null)
            {
                meeting.Property("StartTime").CurrentValue = meeting.Entity.TimeRange.Start;
                meeting.Property("EndTime").CurrentValue = meeting.Entity.TimeRange.End;
            }
        }
    }
}
