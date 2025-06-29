namespace Domain.Entities;

public class Post : BaseEntity
{
    public string? Text { get; set; } = "";

    public Guid UserId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User User { get; set; }
    
    public virtual ICollection<PostLike> PostLikes { get; set; }
}
