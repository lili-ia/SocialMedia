using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class EmailConfirmationTokenConfiguration : IEntityTypeConfiguration<EmailConfirmationToken>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationToken> builder)
    {
        builder.ToTable("EmailConfirmationTokens");
        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(t => t.ExpiresAt)
            .IsRequired();

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}