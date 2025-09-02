using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("Follows");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FollowedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasOne(f => f.Follower)
            .WithMany(u => u.Followees) 
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(f => f.Followee)
            .WithMany(u => u.Followers) 
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(f => new { f.FollowerId, f.FolloweeId })
            .IsUnique();
    }
}