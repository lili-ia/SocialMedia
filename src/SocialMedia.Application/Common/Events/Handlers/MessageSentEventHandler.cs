using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Notification;
using SocialMedia.Application.Mappers;
using SocialMedia.Application.Notifications.Models;

namespace SocialMedia.Application.Common.Events.Handlers;

public sealed class MessageSentEventHandler(
    ICacheService cache,
    IChatRepository chatRepository,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    IMessageRepository messageRepository,
    IRealtimeService realtimeService) : INotificationHandler<MessageSentNotification>
{
    public async Task Handle(MessageSentNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;
        
        await cache.RemoveByPrefixAsync($"messages:chat:{e.ChatId}");  
        
        var chat = await chatRepository.GetByIdWithParticipantsAsync(e.ChatId, ct);
        
        if (chat is null)
        {
            return;
        }

        var sender = await userRepository.GetByIdAsync(e.SenderId, ct);

        if (sender is null)
        {
            return;
        }

        var text = e.Text?.Trim() ?? string.Empty;
        var hasText = !string.IsNullOrWhiteSpace(text);
        var hasAttachments = e.AttachmentsCount > 0;

        string preview;

        switch (hasText)
        {
            case false when hasAttachments:
                preview = $"{e.AttachmentsCount} attachment{(e.AttachmentsCount > 1 ? "s" : "")}";
                break;
            case true when !hasAttachments:
                preview = text.Length <= 60 ? text : $"{text[..60].TrimEnd()}...";
                break;
            case true when hasAttachments:
            {
                var snippet = text.Length <= 45 ? text : $"{text[..45].TrimEnd()}...";
                preview = $"{e.AttachmentsCount} attachment{(e.AttachmentsCount > 1 ? "s" : "")}\n{snippet}";
                break;
            }
            default:
                preview = "Empty message"; 
                break;
        }

        var payload = new NewMessageNotificationData(
            e.ChatId,
            e.MessageId,
            e.SenderId,
            sender.UsernameNormalized,
            preview);

        var serialized = JsonSerializer.Serialize(payload);

        var recipients = chat.Participants
            .Where(p => p.IsActive && p.UserId != e.SenderId)
            .Select(p => p.UserId);

        List<Notification> notifications = [];
        
        foreach (var recipientId in recipients)
        {
            var notif = Notification.Create(
                NotificationType.NewMessage, 
                serialized, 
                recipientId, 
                e.SenderId, 
                e.MessageId);
            
            notifications.Add(notif);
        }

        await notificationRepository.AddRangeAsync(notifications, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var messageDto = await messageRepository.GetByIdAsync(e.MessageId, MessageMapper.ProjectToMessageDto, ct);

        if (messageDto is null)
        {
            return;
        }
        
        await realtimeService.PushMessageAsync(e.ChatId, messageDto, ct);

        foreach (var notif in notifications)
        {
            await realtimeService.PushNotificationAsync(notif.RecipientId, new NotificationDto
            {
                Id = notif.Id,
                Type = notif.Type,
                Payload = notif.Data,
                IsRead = false,
                CreatedAt = notif.CreatedAt
            }, ct);
        }
    } 
}