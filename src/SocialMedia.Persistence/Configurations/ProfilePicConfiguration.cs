using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class ProfilePicConfiguration : IEntityTypeConfiguration<ProfilePic>
{
    public void Configure(EntityTypeBuilder<ProfilePic> builder)
    {
        builder.Property(p => p.FileName)
            .IsRequired()
            .HasMaxLength(255);
    
        builder.Property(p => p.StorageKey)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(p => p.ThumbnailStorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(p => p.User)
            .WithMany() 
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.UserId);
        
        builder.HasIndex(p => p.StorageKey)
            .IsUnique();
        
        builder.HasIndex(p => p.ThumbnailStorageKey)
            .IsUnique();
    }
}