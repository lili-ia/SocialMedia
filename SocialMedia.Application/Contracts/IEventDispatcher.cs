namespace SocialMedia.Application.Contracts;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event);
}