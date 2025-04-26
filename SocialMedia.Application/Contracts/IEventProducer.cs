namespace SocialMedia.Application.Contracts;

public interface IEventProducer
{
    Task SendMessageAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken);
}