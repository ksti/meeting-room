using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeetingRoom.Domain.Aggregates.UserAggregate;

namespace MeetingRoom.Infrastructure.Persistence.Configurations;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.AccessToken)
            .HasMaxLength(500)
            .IsRequired();
        
        builder.Property(t => t.RefreshToken)
            .HasMaxLength(500);
        
        builder.Property(t => t.TokenType)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(t => t.AccessTokenExpiresAt)
            .IsRequired();
        
        builder.Property(t => t.RefreshTokenExpiresAt);
        
        builder.Property(t => t.IsRevoked)
            .IsRequired();
        
        builder.Property(t => t.RevokedAt);
        
        builder.Property(t => t.UserId)
            .IsRequired();
        
        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
