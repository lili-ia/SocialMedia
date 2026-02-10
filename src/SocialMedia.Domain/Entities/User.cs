using Domain.Enums;

namespace Domain.Entities;

public sealed class User : BaseEntity
{
    public string UsernameNormalized { get; set; } = null!;

    public string EmailNormalized { get; set; } = null!;
    
    public DateOnly BirthDate { get; set; }
   
    public string PasswordHash { get; set; } = null!;

    public UserStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                _status = value;
                StatusChangedAt = DateTime.UtcNow;
            }
        }
    } 
    
    public UserRole UserRole { get; set; } = UserRole.User;
    
    public string? Bio { get; set; }
    
    public DateTime? LastSeen { get; set; }
    
    public Guid? CurrentProfilePicId { get; set; }
    
    public ProfilePic? CurrentProfilePic { get; set; }
    
    public DateTime? StatusChangedAt { get; private set; }
    
    public string? StatusReason { get; set; }
    
    public DateTime? LastEmailSentAt { get; set; }

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

    private UserStatus _status = UserStatus.Pending;
}