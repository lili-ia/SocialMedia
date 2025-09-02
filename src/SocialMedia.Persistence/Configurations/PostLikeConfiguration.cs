using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ToTable("PostLikes");

        builder.HasKey(pl => pl.Id);

        builder.Property(pl => pl.LikedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasOne(pl => pl.User)
            .WithMany(u => u.PostLikes) 
            .HasForeignKey(pl => pl.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(pl => pl.Post)
            .WithMany(p => p.PostLikes)
            .HasForeignKey(pl => pl.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(pl => new { pl.UserId, pl.PostId })
            .IsUnique();
    }
}