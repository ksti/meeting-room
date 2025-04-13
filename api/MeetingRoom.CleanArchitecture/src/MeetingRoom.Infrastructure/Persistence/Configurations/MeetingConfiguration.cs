using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Aggregates.RoomAggregate;
using MeetingRoom.Domain.Aggregates.UserAggregate;

namespace MeetingRoom.Infrastructure.Persistence.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Title)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(m => m.Description)
            .HasMaxLength(500);
        
        // 配置 TimeRange 值对象
        builder.Property(m => m.TimeRange.Start)
            .HasColumnName("StartTime")
            .IsRequired();
        
        builder.Property(m => m.TimeRange.End)
            .HasColumnName("EndTime")
            .IsRequired();
        
        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey("RoomId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey("OrganizerId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        // 配置 ParticipantIds 集合，存储为 JSON 数组
        builder.Property(m => m.ParticipantIds)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnName("ParticipantIds");
        
        // 忽略领域事件，因为它们不需要持久化
        builder.Ignore(m => m.DomainEvents);
    }
}
