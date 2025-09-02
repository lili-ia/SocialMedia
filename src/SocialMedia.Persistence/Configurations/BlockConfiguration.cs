using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.ToTable("Blocks");
        
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BlockedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasOne(b => b.Blocker)
            .WithMany() 
            .HasForeignKey(b => b.BlockerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(b => b.Blocked)
            .WithMany() 
            .HasForeignKey(b => b.BlockedId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(b => new { b.BlockerId, b.BlockedId }).IsUnique(); 
    }
}