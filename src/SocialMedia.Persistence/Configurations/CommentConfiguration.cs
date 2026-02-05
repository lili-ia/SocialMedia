using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId);

        builder.HasOne(c => c.ParentComment)
            .WithMany(pc => pc.Replies)
            .HasForeignKey(c => c.ParentCommentId);

        builder.HasIndex(c => new { c.PostId, c.ParentCommentId, c.CreatedAt });

        builder.HasIndex(c => c.ParentCommentId);
    }
}