namespace Domain.Entities;

public class Comment : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    
    public Guid PostId { get; set; }
    
    public Post Post { get; set; } = null!;
    
    public string Text { get; set; } = null!;
    
    public Guid? ParentCommentId { get; set; }
    
    public Comment? ParentComment { get; set; }
    
    public ICollection<Comment> Replies { get; set; } = [];
}