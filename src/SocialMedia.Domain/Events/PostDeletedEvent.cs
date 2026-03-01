namespace Domain.Events;

public class PostDeletedEvent : DomainEvent
{
    public Guid PostId { get; }
    
    public Guid UserId { get; }

    public PostDeletedEvent(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
    }
}