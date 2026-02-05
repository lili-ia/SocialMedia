using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(m => m.Content)
            .HasMaxLength(2000);

        builder.HasOne(m => m.ParentMessage)
            .WithMany()
            .HasForeignKey(m => m.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Attachments)
            .WithOne(a => a.Message)
            .HasForeignKey(a => a.MessageId);
        
        builder.HasIndex(m => new { m.ChatId, m.CreatedAt });

        builder.HasIndex(m => new { m.ChatId, m.IsRead })
            .HasFilter("\"IsRead\" = false");
    }
}
