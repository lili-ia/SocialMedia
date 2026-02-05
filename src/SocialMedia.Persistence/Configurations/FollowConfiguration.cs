using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.HasIndex(f => new { f.FollowerId, f.FolloweeId })
            .IsUnique();
    }
}