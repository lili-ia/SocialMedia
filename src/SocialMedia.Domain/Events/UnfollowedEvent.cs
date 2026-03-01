namespace Domain.Events;

public class UnfollowedEvent : DomainEvent
{
    public Guid FollowerId { get; }
    
    public Guid FolloweeId { get; }
    
    public UnfollowedEvent(Guid followerId, Guid followeeId)
    {
        FollowerId = followerId;
        FolloweeId = followeeId;
    }
}