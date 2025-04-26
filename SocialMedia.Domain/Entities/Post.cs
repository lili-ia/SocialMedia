namespace Domain.Entities;

public class Post
{
    public int PostId { get; set; }

    public string? Text { get; set; }

    public int UserId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? User { get; set; }
    
    public virtual ICollection<PostLike> PostLikes { get; set; }
}
