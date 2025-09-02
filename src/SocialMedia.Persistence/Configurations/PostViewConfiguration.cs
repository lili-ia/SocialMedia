using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostViewConfiguration : IEntityTypeConfiguration<PostView>
{
    public void Configure(EntityTypeBuilder<PostView> builder)
    {
        builder.ToTable("PostViews");

        builder.HasKey(pv => pv.Id);

        builder.Property(pv => pv.ViewedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasOne(pv => pv.User)
            .WithMany(u => u.PostViews)
            .HasForeignKey(pv => pv.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(pv => pv.Post)
            .WithMany(p => p.PostViews)
            .HasForeignKey(pv => pv.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(pv => new { pv.UserId, pv.PostId })
            .IsUnique();
    }
}