using System.Text.Json;
using Confluent.Kafka;
using Domain.Events;
using Microsoft.Extensions.Hosting;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Messaging.Consumers;

public class KafkaNotificationConsumer : BackgroundService
{
    private IConsumer<string, string> _consumer;
    private INotificationService _notificationService;

    public KafkaNotificationConsumer(
        IConsumer<string, string> consumer, 
        INotificationService notificationService)
    {
        _consumer = consumer;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(new[]
        {
            "likes-topic", "comments-topic", "messages-topic", "follows-topic"
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _consumer.Consume(stoppingToken);

            if (result != null)
            {
                var topic = result.Topic;

                var notification = JsonSerializer
                    .Deserialize<NotificationEvent>(result.Message.Value);

                switch (notification.Type)
                {
                    case "PostLiked":
                    {
                        var liked = JsonSerializer
                            .Deserialize<PostLikedEvent>(result.Message.Value);
                        await _notificationService.NotifyPostLiked(liked);
                        break;
                    }
                    case "UserFollowed":
                    {
                        var followed = JsonSerializer
                            .Deserialize<FollowedEvent>(result.Message.Value);
                        await _notificationService.NotifyUserFollowed(followed);
                        break;
                    }
                    case "MessageReceived":
                    {
                        var messageReceived = JsonSerializer
                            .Deserialize<MessageReceivedEvent>(result.Message.Value);
                        await _notificationService.NotifyMessageReceived(messageReceived);
                        break;
                    }
                    case "PostCommented":
                    {
                        var commented = JsonSerializer
                            .Deserialize<PostCommentedEvent>(result.Message.Value);
                        await _notificationService.NotifyPostCommented(commented);
                        break;
                    }
                }
            }
        }
        
        
    }
}