using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.ToTable("ChatParticipants");
        
        builder.HasKey(cp => new { cp.ChatId, cp.UserId }); 

        builder.Property(cp => cp.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasOne(cp => cp.Chat)
            .WithMany(c => c.ChatParticipants)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(cp => cp.User)
            .WithMany() 
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}