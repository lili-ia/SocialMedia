using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostViewConfiguration : IEntityTypeConfiguration<PostView>
{
    public void Configure(EntityTypeBuilder<PostView> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.HasIndex(pv => new { pv.UserId, pv.PostId })
            .IsUnique();
    }
}