using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class PostFileConfiguration : IEntityTypeConfiguration<PostFile>
{
    public void Configure(EntityTypeBuilder<PostFile> builder)
    {
        builder.ToTable("PostFiles");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.Url)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(f => f.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasIndex(f => f.PostId);

        builder.HasOne(f => f.Post)
            .WithMany(p => p.PostFiles)
            .HasForeignKey(f => f.PostId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}