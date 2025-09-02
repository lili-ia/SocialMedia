using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Type)
            .IsRequired();

        builder.Property(n => n.IsRead)
            .HasDefaultValue(false);

        builder.Property(n => n.Timestamp)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(n => n.Data)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new()
            );

        builder.HasOne(n => n.Recipient)
            .WithMany(u => u.Notifications) 
            .HasForeignKey(n => n.RecipientId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}