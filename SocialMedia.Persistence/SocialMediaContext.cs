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
    
    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    
    public virtual DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }
    
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
                .OnDelete(DeleteBehavior.SetNull);

            // User -> Messages (one-to-many)
            entity.HasMany(u => u.Messages)
                .WithOne(m => m.Sender) 
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.SetNull);

            // User -> Posts (one-to-many)
            entity.HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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
            
            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.ProfilePicUrl)
                .HasMaxLength(255);

            entity.Property(u => u.Bio)
                .HasMaxLength(500);

            entity.HasCheckConstraint("CK_User_BirthDate", "BirthDate <= GETDATE()");
            
            entity.HasIndex(u => u.Email).IsUnique();
            
            entity.HasIndex(u => u.Username).IsUnique();

        });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasCheckConstraint("CK_Chat_Title_NotEmpty", "LEN(Title) > 0");
        });

        
        modelBuilder.Entity<Message>(entity =>
        {
            // Message -> Chat (many-to-one)
            entity.HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(m => m.Timestamp)
                .HasDefaultValueSql("GETDATE()");
        });
        
        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(pl => new { pl.UserId, pl.PostId });
            
            entity.Property(pl => pl.LikedAt)
                .HasDefaultValueSql("GETDATE()");

            // Relationship to User
            entity.HasOne(pl => pl.User)
                .WithMany(u => u.PostLikes)
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship to Post
            entity.HasOne(pl => pl.Post)
                .WithMany(p => p.PostLikes)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(c => c.Text)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });

        
        modelBuilder.Entity<Follow>(entity =>
        {
            entity.HasKey(f => new { f.FollowerId, f.FolloweeId });
            
            entity.HasOne(f => f.Follower)
                .WithMany(u => u.Followees)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(f => f.FollowedAt)
                .HasDefaultValueSql("GETDATE()");
        });
        
        modelBuilder.Entity<Post>(entity =>
        {
            // Post - Comments (one-to-many)
            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Post) 
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(p => p.Text)
                .HasMaxLength(2000);

            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });
        
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(n => n.Data)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null))
                .HasColumnType("nvarchar(max)");
        });
        
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(rt => rt.IpAddress)
                .HasMaxLength(45);

            entity.Property(rt => rt.DeviceInfo)
                .HasMaxLength(500);

            entity.Property(rt => rt.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.Property(x => x.UserId)
                .IsRequired();

            entity.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(256);  

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.ExpiresAt)
                .IsRequired();

            entity.Property(x => x.IsUsed)
                .IsRequired();

            entity.HasOne(x => x.User)
                .WithMany()         
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<EmailConfirmationToken>(entity =>
        {
            entity.Property(x => x.UserId)
                .IsRequired();

            entity.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(256);  

            entity.Property(x => x.CreatedAt)
                .IsRequired();

            entity.Property(x => x.ExpiresAt)
                .IsRequired();

            entity.Property(x => x.IsUsed)
                .IsRequired();

            entity.HasOne(x => x.User)
                .WithMany()         
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
