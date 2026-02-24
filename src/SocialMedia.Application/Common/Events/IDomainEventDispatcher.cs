using Domain.Events;

namespace SocialMedia.Application.Common.Events;

public interface IDomainEventDispatcher
{ 
    Task DispatchEventsAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}