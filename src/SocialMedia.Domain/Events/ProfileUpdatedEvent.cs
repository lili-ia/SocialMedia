namespace Domain.Events;

public class ProfileUpdatedEvent : DomainEvent
{
    public ProfileUpdatedEvent(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}