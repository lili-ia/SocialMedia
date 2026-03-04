namespace Domain.Events;

public class PostUpdatedEvent : DomainEvent
{
    public PostUpdatedEvent(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
    }

    public Guid PostId { get; }
    
    public Guid UserId { get; }
}