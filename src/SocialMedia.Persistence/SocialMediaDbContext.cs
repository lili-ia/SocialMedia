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
    
    public DbSet<Block> Blocks { get; set; }

    public DbSet<Chat> Chats { get; set; }
    
    public DbSet<ChatParticipant> ChatParticipants { get; set; }

    public DbSet<Comment> Comments { get; set; }
    
    public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }
    
    public DbSet<Follow> Follows { get; set; }
    
    public DbSet<Message> Messages { get; set; }
    
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    
    public DbSet<Notification> Notifications { get; set; }
    
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    
    public DbSet<PendingEmail> PendingEmails { get; set; }

    public DbSet<Post> Posts { get; set; }
    
    public DbSet<PostFile> PostFiles { get; set; }
    
    public DbSet<PostLike> PostLikes { get; set; }

    public DbSet<PostView> PostViews { get; set; }
    
    public DbSet<ProfilePic> ProfilePics { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
    }
}
