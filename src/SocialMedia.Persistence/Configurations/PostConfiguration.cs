using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Text)
            .HasMaxLength(2000);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.CreatedAt);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Posts)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.PostLikes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.PostFiles)
            .WithOne(f => f.Post)
            .HasForeignKey(f => f.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.PostViews)
            .WithOne(v => v.Post)
            .HasForeignKey(v => v.PostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}