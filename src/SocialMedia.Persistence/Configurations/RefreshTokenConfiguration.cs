using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(rt => rt.IpAddress)
            .HasMaxLength(100);

        builder.Property(rt => rt.DeviceInfo)
            .HasMaxLength(255);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId);
    }
}

