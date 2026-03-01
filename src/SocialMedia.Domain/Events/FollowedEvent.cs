namespace Domain.Events;

public class FollowedEvent : DomainEvent
{
    public Guid FollowId { get; set; }
    
    public Guid FollowerId { get; }
    
    public Guid FolloweeId { get; }
    
    public FollowedEvent(Guid followId, Guid followerId, Guid followeeId)
    {
        FollowId = followId;
        FollowerId = followerId;
        FolloweeId = followeeId;
    }
}