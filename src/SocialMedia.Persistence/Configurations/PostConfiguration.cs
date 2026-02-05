using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(p => p.Text)
            .HasMaxLength(2000);
        
        builder.HasIndex(p => new { p.UserId, p.CreatedAt, p.IsHidden })
            .HasFilter("\"IsHidden\" = false");
        
        builder.Property(p => p.LikeCount).HasDefaultValue(0);
        
        builder.Property(p => p.CommentCount).HasDefaultValue(0);
        
        builder.Property(p => p.ViewCount).HasDefaultValue(0);

        builder.HasMany(p => p.PostLikes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.PostFiles)
            .WithOne(f => f.Post)
            .HasForeignKey(f => f.PostId);

        builder.HasMany(p => p.PostViews)
            .WithOne(v => v.Post)
            .HasForeignKey(v => v.PostId);
    }
}