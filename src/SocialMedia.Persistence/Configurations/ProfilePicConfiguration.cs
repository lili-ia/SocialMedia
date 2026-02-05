using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class ProfilePicConfiguration : IEntityTypeConfiguration<ProfilePic>
{
    public void Configure(EntityTypeBuilder<ProfilePic> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(p => p.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);
    
        builder.Property(p => p.OriginalStorageKey)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(p => p.ThumbnailStorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(p => p.User)
            .WithOne(u => u.CurrentProfilePic)
            .HasForeignKey<ProfilePic>(p => p.UserId);

        builder.HasIndex(p => p.UserId);
        
        builder.HasIndex(p => p.OriginalStorageKey)
            .IsUnique();
        
        builder.HasIndex(p => p.ThumbnailStorageKey)
            .IsUnique();
    }
}