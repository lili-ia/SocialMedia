namespace Domain.Entities;

public class Comment
{
    public int CommentId { get; set; }

    public string? Text { get; set; }

    public int? UserId { get; set; }

    public int? PostId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
