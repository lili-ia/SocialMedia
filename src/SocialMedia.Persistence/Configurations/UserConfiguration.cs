using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.UsernameNormalized)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(u => u.UsernameNormalized).IsUnique();
        
        builder.Property(u => u.EmailNormalized)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(u => u.EmailNormalized)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .IsRequired();

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.Version)
            .IsRowVersion();

        builder.HasOne(u => u.CurrentProfilePic)
            .WithOne(p => p.User);

        builder.HasMany(u => u.Messages)
            .WithOne(m => m.Sender);

        builder.HasMany(u => u.Posts)
            .WithOne(p => p.User);

        builder.HasMany(u => u.Followees)
            .WithOne(f => f.Follower)
            .HasForeignKey(f => f.FollowerId);

        builder.HasMany(u => u.Followers)
            .WithOne(f => f.Followee)
            .HasForeignKey(f => f.FolloweeId);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(t => t.User);
        
        builder.HasMany(u => u.PostLikes)
            .WithOne(l => l.User);
        
        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.Recipient);
        
        builder.HasMany(u => u.PostViews)
            .WithOne(v => v.User);
        
        builder.HasMany(u => u.BlockedUsers)
            .WithOne(f => f.Blocker)
            .HasForeignKey(f => f.BlockerId);
        
        builder.HasMany(u => u.BlockedByUsers)
            .WithOne(f => f.Blocked)
            .HasForeignKey(f => f.BlockedId);
    }
}
