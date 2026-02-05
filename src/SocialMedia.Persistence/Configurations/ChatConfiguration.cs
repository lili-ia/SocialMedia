using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(c => c.Title)
            .HasMaxLength(200);

        builder.HasOne(c => c.LastMessage)
            .WithMany()
            .HasForeignKey(c => c.LastMessageId);

        builder.HasIndex(c => c.LastActivityAt);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Chat)
            .HasForeignKey(m => m.ChatId);

        builder.HasMany(c => c.ChatParticipants)
            .WithOne(cp => cp.Chat)
            .HasForeignKey(cp => cp.ChatId);
    }
}