using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.ValueObjects;

namespace MeetingRoom.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(100)
                .IsRequired();
            
            email.HasIndex(e => e.Value)
                .IsUnique();
        });
        
        builder.OwnsOne(u => u.Password, password =>
        {
            password.Property(p => p.PasswordHash)
                .HasColumnName("PasswordHash")
                .HasMaxLength(100)
                .IsRequired();
        });
        
        // 配置用户角色和状态
        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();
            
        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>();
            
        // 配置可空属性
        builder.Property(u => u.Contact)
            .HasMaxLength(50);
            
        builder.Property(u => u.Avatar)
            .HasMaxLength(255);
            
        // 配置 PersonName 值对象
        builder.OwnsOne(u => u.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(50);
                
            name.Property(n => n.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(50);
        });
        
        // 配置导航属性
        builder.HasMany(u => u.Tokens)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(u => u.Devices)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
        
        // 忽略领域事件，因为它们不需要持久化
        builder.Ignore(u => u.DomainEvents);
    }
}
