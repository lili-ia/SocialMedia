using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.HasIndex(pl => new { pl.UserId, pl.PostId })
            .IsUnique();
    }
}