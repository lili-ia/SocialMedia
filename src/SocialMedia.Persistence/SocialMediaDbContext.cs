using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Persistence.Configurations;

namespace SocialMedia.Persistence;

public class SocialMediaDbContext : DbContext
{
    public SocialMediaDbContext()
    {
    }

    public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options)
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
    
    public virtual DbSet<PostView> PostViews { get; set; }
    
    public virtual DbSet<Follow> Follows { get; set; }
    
    public virtual DbSet<Notification> Notifications { get; set; }
    
    public virtual DbSet<Block> Blocks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}
