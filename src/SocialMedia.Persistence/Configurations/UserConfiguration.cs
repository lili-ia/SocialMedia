using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Status)
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .IsRequired();

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(u => u.LastSeen);

        builder.Property(u => u.UserRole)
            .HasDefaultValue(UserRole.User);

        builder.HasIndex(u => u.Username).IsUnique();
        
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
