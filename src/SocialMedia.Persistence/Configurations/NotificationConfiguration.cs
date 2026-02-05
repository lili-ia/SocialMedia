using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(n => n.Data)
            .HasColumnType("jsonb");
        
        builder.HasIndex(n => new { n.RecipientId, n.IsRead })
            .HasFilter("\"IsRead\" = false");

        builder.HasIndex(n => new { n.RecipientId, n.CreatedAt });
    }
}