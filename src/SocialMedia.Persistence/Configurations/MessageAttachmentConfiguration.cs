using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> builder)
    {
        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);
    
        builder.Property(a => a.StorageKey)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.HasIndex(a => a.MessageId);

        builder.HasOne(a => a.Message)
            .WithMany(m => m.Attachments)
            .HasForeignKey(a => a.MessageId);
        
        builder.HasIndex(f => f.StorageKey)
            .IsUnique();
    }
}