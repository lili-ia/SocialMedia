using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public Guid? ProfilePicId { get; set; }
    
    public string? Bio { get; set; }

    public UserStatus Status { get; set; } 

    public DateTime? LastSeen { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public UserRole UserRole { get; set; } = UserRole.User;
    
    public ProfilePic? ProfilePic { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    
    public ICollection<Follow> Followees { get; set; } = new List<Follow>();
    
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
    public ICollection<PostView> PostViews { get; set; } = new List<PostView>();
    
    public ICollection<Block> BlockedUsers { get; set; } = new List<Block>();
}