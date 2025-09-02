using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
   
    public string PasswordHash { get; set; } = null!;
    
    public UserStatus Status { get; set; } 
    
    public DateTime BirthDate { get; set; }
    
    public string? Bio { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastSeen { get; set; }

    public UserRole UserRole { get; set; } = UserRole.User;
    
    public Guid? ProfilePicId { get; set; }
    
    public ProfilePic? ProfilePic { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
    
    public ICollection<Message> Messages { get; set; } = [];
    
    public ICollection<Post> Posts { get; set; } = [];
    
    public ICollection<Follow> Followees { get; set; } = [];
    
    public ICollection<Follow> Followers { get; set; } = [];
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    
    public ICollection<PostLike> PostLikes { get; set; } = [];
    
    public ICollection<Notification> Notifications { get; set; } = [];
    
    public ICollection<PostView> PostViews { get; set; } = [];
    
    public ICollection<Block> BlockedUsers { get; set; } = [];
    
    public ICollection<Block> BlockedByUsers { get; set; } = [];
}