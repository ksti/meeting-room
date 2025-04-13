using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetingRoom.Domain.Aggregates.UserAggregate;

namespace MeetingRoom.Infrastructure.Persistence.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(d => d.Id);
        
        // 配置 DeviceInfo 值对象
        builder.OwnsOne(d => d.DeviceInfo, di => 
        {
            di.Property(i => i.DeviceIdentifier)
                .HasMaxLength(100)
                .IsRequired();
                
            di.Property(i => i.DeviceName)
                .HasMaxLength(100);
                
            di.Property(i => i.Platform)
                .HasMaxLength(50);
                
            di.Property(i => i.OperatingSystem)
                .HasMaxLength(50);
                
            di.Property(i => i.OsVersion)
                .HasMaxLength(50);
        });
        
        // Device 实体不包含领域事件，所以不需要忽略
    }
}
