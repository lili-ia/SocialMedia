namespace Domain.Events;

public class PostCreatedEvent : DomainEvent
{
    public PostCreatedEvent(Guid postId, Guid authorId)
    {
        PostId = postId;
        AuthorId = authorId;
    }

    public Guid PostId { get; }
    
    public Guid AuthorId { get; }
}