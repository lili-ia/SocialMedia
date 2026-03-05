using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostFileConfiguration : IEntityTypeConfiguration<PostFile>
{
    public void Configure(EntityTypeBuilder<PostFile> builder)
    {
        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);
    
        builder.Property(f => f.StorageKey)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.HasIndex(f => f.PostId);

        builder.HasOne(f => f.Post)
            .WithMany(p => p.PostFiles)
            .HasForeignKey(f => f.PostId);
        
        builder.HasIndex(f => f.StorageKey)
            .IsUnique();
    }
}