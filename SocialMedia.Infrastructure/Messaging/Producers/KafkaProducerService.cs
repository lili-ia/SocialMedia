using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Messaging.Producers;

public class KafkaProducerService : IEventProducer
{
    private readonly ILogger<KafkaProducerService> _logger;
    
    public KafkaProducerService(ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
    }

    public async Task SendMessageAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var message = new Message<Null, string>
        {
            Value = JsonSerializer.Serialize(@event)
        };
        
        try
        {
            var deliveryResult = await producer.ProduceAsync(topic, message, cancellationToken);
            
            _logger.LogInformation(
                $"Delivered information to {deliveryResult.Value}, Offset: {deliveryResult.Offset}");
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError($"Delivery failed: {e.Error.Reason}");
        }

        producer.Flush(cancellationToken);
    }
}