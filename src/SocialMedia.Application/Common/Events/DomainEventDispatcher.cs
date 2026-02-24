using Domain.Events;
using MediatR;

namespace SocialMedia.Application.Common.Events;

public class DomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchEventsAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken ct)
    {
        foreach (var domainEvent in events)
        {
            await mediator.Publish(domainEvent, ct);
        }
    }
}