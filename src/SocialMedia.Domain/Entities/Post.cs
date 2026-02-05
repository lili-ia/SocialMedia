namespace Domain.Entities;

public sealed class Post : BaseEntity
{
    public string? Text { get; set; }

    public bool IsHidden { get; set; } = false;
    
    public Guid UserId { set; get; }

    public User User { get; set; } = null!;

    public int LikeCount { get; set; }
    
    public int CommentCount { get; set; }
    
    public int ViewCount { get; set; }
    
    public ICollection<Comment> Comments { get; set; } = [];

    public ICollection<PostLike> PostLikes { get; set; } = [];

    public ICollection<PostFile> PostFiles { get; set; } = [];

    public ICollection<PostView> PostViews { get; set; } = [];
}