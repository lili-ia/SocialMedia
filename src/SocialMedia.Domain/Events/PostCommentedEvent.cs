namespace Domain.Events;

public class PostCommentedEvent : DomainEvent
{
    public PostCommentedEvent(Guid commentId, Guid postId, Guid commenterId,string text)
    {
        CommentId = commentId;
        PostId = postId;
        CommenterId = commenterId;
        Text = text;
    }
    
    public Guid CommentId { get; set; }

    public Guid PostId { get; }
    
    public Guid CommenterId { get; }
    
    public string Text { get; }
}