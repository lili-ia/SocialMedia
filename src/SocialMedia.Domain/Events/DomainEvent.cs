namespace Domain.Events;

public abstract class DomainEvent : IDomainEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}