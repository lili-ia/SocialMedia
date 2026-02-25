namespace Domain.Entities;

public class Comment : BaseEntity
{
    public Guid UserId { get; private set; }
    
    public User User { get; private set; } = null!;

    public Guid PostId { get; private set; }
    
    public Post Post { get; private set; } = null!;

    public string Text { get; private set; } = null!;

    public Guid? ParentCommentId { get; private set; }
    
    public Comment? ParentComment { get; private set; }

    public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();

    private Comment() { }

    private Comment(
        Guid userId,
        Guid postId,
        string text,
        Guid? parentCommentId = null)
    {
        UserId = userId;
        PostId = postId;
        Text = text;
        ParentCommentId = parentCommentId;
    }

    public static Comment Create(
        Guid userId,
        Guid postId,
        string text,
        Guid? parentCommentId = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Comment text cannot be empty");
        }

        if (text.Length > 500)
        {
            throw new ArgumentException("Comment text too long");
        }

        return new Comment(userId, postId, text, parentCommentId);
    }

    public void UpdateText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Comment text cannot be empty");
        }

        Text = text;
    }

    public void AddReply(Comment reply)
    {
        if (reply is null)
        {
            throw new ArgumentNullException(nameof(reply));
        }

        _replies.Add(reply);
    }
    
    public bool CanUserDelete(Guid userId)
    {
        return UserId == userId || Post.UserId == userId;
    }
    
    private readonly List<Comment> _replies = [];
}