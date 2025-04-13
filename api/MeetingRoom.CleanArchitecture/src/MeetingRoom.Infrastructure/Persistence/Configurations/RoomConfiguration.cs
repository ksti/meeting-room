using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetingRoom.Domain.Aggregates.RoomAggregate;

namespace MeetingRoom.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(r => r.Description)
            .HasMaxLength(500);
        
        builder.Property(r => r.Capacity)
            .IsRequired();
        
        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();
        
        // 忽略领域事件，因为它们不需要持久化
        builder.Ignore(r => r.DomainEvents);
    }
}
