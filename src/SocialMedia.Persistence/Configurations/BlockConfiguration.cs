using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.HasIndex(b => new { b.BlockerId, b.BlockedId })
            .IsUnique(); 
        
        builder.ToTable(t => 
            t.HasCheckConstraint("CK_Block_NotSelf", "\"BlockerId\" <> \"BlockedId\""));
    }
}