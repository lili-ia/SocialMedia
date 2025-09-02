using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000); 

        builder.Property(m => m.Timestamp)
            .IsRequired()
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(m => m.MessageType)
            .IsRequired();

        builder.Property(m => m.IsEdited)
            .HasDefaultValue(false);

        builder.Property(m => m.IsRead)
            .HasDefaultValue(false);

        builder.Property(m => m.IsDeleted)
            .HasDefaultValue(false);

        builder.HasOne(m => m.Sender)
            .WithMany() 
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.ReplyToMessage)
            .WithMany() 
            .HasForeignKey(m => m.ReplyToMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
