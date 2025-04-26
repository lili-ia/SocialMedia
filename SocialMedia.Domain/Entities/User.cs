namespace Domain.Entities;

public class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? ProfilePicUrl { get; set; }

    public string? Bio { get; set; }

    public int? Status { get; set; }

    public DateTime? LastSeen { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Follow> Followees { get; set; } = new List<Follow>();

    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public virtual ICollection<PostLike> PostLikes { get; set; }
}
