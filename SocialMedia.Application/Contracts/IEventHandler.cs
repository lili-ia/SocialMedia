namespace SocialMedia.Application.Contracts;

public interface IEventHandler<in TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken? cancellationToken = null);
}