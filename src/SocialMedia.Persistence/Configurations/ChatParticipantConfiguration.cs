using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.HasIndex(cp => new { cp.UserId, cp.ChatId })
            .IsUnique();

        builder.HasIndex(cp => new { cp.ChatId, cp.UserId })
            .IncludeProperties(cp => cp.IsAdmin);

        builder.HasOne(cp => cp.Chat)
            .WithMany(c => c.ChatParticipants)
            .HasForeignKey(cp => cp.ChatId);

        builder.HasOne(cp => cp.User)
            .WithMany()
            .HasForeignKey(cp => cp.UserId);
    }
}