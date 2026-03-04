namespace Domain.Events;

public class PostUnlikedEvent : DomainEvent
{
    public PostUnlikedEvent(Guid postId, Guid likerId)
    {
        PostId = postId;
        LikerId = likerId;
    }

    public Guid PostId { get; }
    
    public Guid LikerId { get; }
}