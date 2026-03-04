namespace Domain.Events;

public class PostHiddenStatusChangedEvent : DomainEvent
{
    public PostHiddenStatusChangedEvent(Guid postId, Guid authorId, bool isHidden)
    {
        PostId = postId;
        AuthorId = authorId;
        IsHidden = isHidden;
    }

    public Guid PostId { get; }
    
    public Guid AuthorId { get; }
    
    public bool IsHidden { get;  }
}