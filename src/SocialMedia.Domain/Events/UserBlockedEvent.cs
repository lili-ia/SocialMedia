namespace Domain.Events;

public class UserBlockedEvent : DomainEvent
{
    public Guid BlockerId { get; }
    
    public Guid BlockedId { get; }

    public UserBlockedEvent(
        Guid blockedId, 
        Guid blockerId)
    {
        BlockedId = blockedId;
        BlockerId = blockerId;
    }
}