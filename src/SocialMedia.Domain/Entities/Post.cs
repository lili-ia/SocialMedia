namespace Domain.Entities;

public class Post : BaseEntity
{
    public string? Text { get; set; }

    public Guid UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public User User { get; set; } = null!;
    
    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<PostLike> PostLikes { get; set; } = [];

    public ICollection<PostFile> PostFiles { get; set; } = [];
}