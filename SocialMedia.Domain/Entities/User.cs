using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = "";

    public DateTime BirthDate { get; set; } = DateTime.Now;

    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public string? ProfilePicUrl { get; set; }

    public string? Bio { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Pending;

    public DateTime? LastSeen { get; set; }


    public UserRole UserRole { get; set; } = UserRole.User;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Follow> Followees { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public virtual ICollection<PostLike> PostLikes { get; set; }
}
