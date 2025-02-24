using MeetingRoom.Api.Common;
using MeetingRoom.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MeetingRoom.Api.Data
{
    public class ApplicationDbContext(IConfiguration configuration) : IdentityDbContext<IdentityUser>
    {
        public new DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<TokenEntity> Tokens { get; set; } = null!;
        public DbSet<DeviceEntity> Devices { get; set; } = null!;
        public DbSet<RoomEntity> Rooms { get; set; } = null!;
        public DbSet<MeetingEntity> Meetings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure base entity properties for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("CreatedAt")
                        .IsRequired();

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("CreatedBy")
                        .IsRequired(false);

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("UpdatedBy")
                        .IsRequired(false);

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("IsDeleted")
                        .IsRequired()
                        .HasDefaultValue(false);

                    var property = entityType.FindProperty("IsDeleted");
                    if (property != null && property.ClrType == typeof(bool))
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "p");
                        var left = Expression.Property(parameter, property.PropertyInfo!);
                        Expression<Func<bool>> isDeleted = () => false;
                        var right = isDeleted.Body;
                        var filter = Expression.Lambda(Expression.Equal(left, right), parameter);
                        
                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                    }
                }
            }

            // Users
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.PasswordHash).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Tokens
            modelBuilder.Entity<TokenEntity>(entity =>
            {
                entity.ToTable("Tokens");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.AccessToken).IsUnique();

                entity.Property(e => e.AccessToken).IsRequired().HasMaxLength(500);
                entity.Property(e => e.RefreshToken).HasMaxLength(500);
                entity.Property(e => e.TokenType).HasMaxLength(50);
                entity.Property(e => e.AccessTokenExpiresAt);
                entity.Property(e => e.RefreshTokenExpiresAt);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Tokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            // Devices
            modelBuilder.Entity<DeviceEntity>(entity =>
            {
                entity.ToTable("Devices");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DeviceIdentifier);
                entity.HasIndex(e => e.Platform);

                entity.Property(e => e.DeviceIdentifier).IsRequired().HasMaxLength(100);
                entity.Property(e => e.DeviceName).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Platform).HasMaxLength(50);
                entity.Property(e => e.OperatingSystem).HasMaxLength(50);
                entity.Property(e => e.OsVersion).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Devices)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Meetings
            modelBuilder.Entity<MeetingEntity>(entity =>
            {
                entity.ToTable("Meetings");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Title);

                entity.Property(e => e.Title).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Capacity);
                entity.Property(e => e.StartTime);
                entity.Property(e => e.EndTime);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(e => e.Room)
                    .WithMany(e => e.Meetings)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Attendees)
                    .WithMany(e => e.Meetings)
                    .UsingEntity(j => j.ToTable("MeetingUsers"));
            });

            // Rooms
            modelBuilder.Entity<RoomEntity>(entity =>
            {
                entity.ToTable("Rooms");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name);

                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Capacity);
                entity.Property(e => e.Status).HasMaxLength(20);
            });
        }
    }
}
