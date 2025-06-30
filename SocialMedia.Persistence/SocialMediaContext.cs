using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Persistence;

public partial class SocialMediaContext : DbContext
{
    public SocialMediaContext()
    {
    }

    public SocialMediaContext(DbContextOptions<SocialMediaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public virtual DbSet<PostLike> PostLikes { get; set; }
    
    public virtual DbSet<Follow> Follows { get; set; }
    
    public virtual DbSet<Notification> Notifications { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            // User -> Comments (one-to-many)
            entity.HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Messages (one-to-many)
            entity.HasMany(u => u.Messages)
                .WithOne(m => m.Sender) 
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Posts (one-to-many)
            entity.HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Followees (one-to-many)
            entity.HasMany(u => u.Followees)
                .WithOne(f => f.Follower)  
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> Followers (one-to-many)
            entity.HasMany(u => u.Followers)
                .WithOne(f => f.Followee)  
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> RefreshTokens (one-to-many)
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // User -> Notifications (one-to-many)
            entity.HasMany(u => u.Notifications)
                .WithOne(rt => rt.Recipient)
                .HasForeignKey(rt => rt.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            // Message -> Chat (many-to-one)
            entity.HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(pl => new { pl.UserId, pl.PostId });

            // Relationship to User
            entity.HasOne(pl => pl.User)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship to Post
            entity.HasOne(pl => pl.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Post>(entity =>
        {
            // Post - Comments (one-to-many)
            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Post) 
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(n => n.Data)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null))
                .HasColumnType("nvarchar(max)");
        });
    }
}
