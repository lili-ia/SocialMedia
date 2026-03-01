namespace Domain.Events;

public class CommentDeletedEvent : IDomainEvent
{
    public CommentDeletedEvent(Guid commentId)
    {
        CommentId = commentId;
    }

    public Guid CommentId { get; }
}