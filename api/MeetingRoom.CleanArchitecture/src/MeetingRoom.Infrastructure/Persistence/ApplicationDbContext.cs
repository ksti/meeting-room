using Microsoft.EntityFrameworkCore;
using MeetingRoom.Application.Interfaces;
using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Aggregates.RoomAggregate;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.Shared;
using MeetingRoom.Domain.ValueObjects;
using MeetingRoom.Infrastructure.Persistence.Interceptors;
using System.Reflection;

namespace MeetingRoom.Infrastructure.Persistence;

/// <summary>
/// 应用程序数据库上下文
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly IDateTime _dateTime;
    private readonly ICurrentUserService _currentUserService;
    
    private readonly TimeRangeInterceptor _timeRangeInterceptor;
    
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDateTime dateTime,
        ICurrentUserService currentUserService,
        TimeRangeInterceptor timeRangeInterceptor) : base(options)
    {
        _dateTime = dateTime;
        _currentUserService = currentUserService;
        _timeRangeInterceptor = timeRangeInterceptor;
    }
    
    /// <summary>
    /// 用户
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// 会议室
    /// </summary>
    public DbSet<Room> Rooms { get; set; }
    
    /// <summary>
    /// 会议
    /// </summary>
    public DbSet<Meeting> Meetings { get; set; }
    
    // 注意：在领域模型中，参与者作为 Meeting 聚合根的一部分，通过 ParticipantIds 集合来表示
    // 不再需要单独的 Participants DbSet
    
    /// <summary>
    /// 设备
    /// </summary>
    public DbSet<Device> Devices { get; set; }
    
    /// <summary>
    /// 令牌
    /// </summary>
    public DbSet<Token> Tokens { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 添加拦截器
        optionsBuilder.AddInterceptors(_timeRangeInterceptor);
        
        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 应用所有实体配置
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 自动设置审计属性
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = _dateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserId ?? "system";
                    break;
                
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = _dateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserService.UserId ?? "system";
                    break;
            }
        }
        
        // 处理 Meeting 实体的 TimeRange 值对象
        SetTimeRangeValues();
        
        // 处理领域事件
        var entitiesWithEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToArray();
            
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            
            foreach (var domainEvent in events)
            {
                // 这里应该分发领域事件
                // 在实际应用中，可以使用MediatR或其他事件总线
                // 简化起见，这里不做实现
            }
        }
        
        var result = await base.SaveChangesAsync(cancellationToken);
        
        // 加载后处理 TimeRange 值对象
        LoadTimeRangeValues();
        
        return result;
    }
    
    /// <summary>
    /// 从数据库加载后，根据影子属性设置 TimeRange 值对象
    /// </summary>
    private void LoadTimeRangeValues()
    {
        var meetings = ChangeTracker.Entries<Meeting>()
            .Where(e => e.State == EntityState.Unchanged || e.State == EntityState.Modified)
            .ToList();
        
        foreach (var meeting in meetings)
        {
            var startTime = (DateTime)meeting.Property("StartTime").CurrentValue;
            var endTime = (DateTime)meeting.Property("EndTime").CurrentValue;
            
            // 使用反射设置私有字段，因为 TimeRange 是不可变的值对象
            var timeRange = TimeRange.Create(startTime, endTime);
            var property = typeof(Meeting).GetProperty("TimeRange");
            property?.SetValue(meeting.Entity, timeRange);
        }
    }
    
    /// <summary>
    /// 保存到数据库前，将 TimeRange 值对象的值设置到影子属性
    /// </summary>
    private void SetTimeRangeValues()
    {
        var meetings = ChangeTracker.Entries<Meeting>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();
        
        foreach (var meeting in meetings)
        {
            if (meeting.Entity.TimeRange != null)
            {
                meeting.Property("StartTime").CurrentValue = meeting.Entity.TimeRange.Start;
                meeting.Property("EndTime").CurrentValue = meeting.Entity.TimeRange.End;
            }
        }
    }
}
