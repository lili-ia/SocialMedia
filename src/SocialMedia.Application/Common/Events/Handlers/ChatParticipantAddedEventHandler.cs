using System.Text.Json;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using SocialMedia.Application.Common.Events.EventWrappers;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.Notifications.Models;

namespace SocialMedia.Application.Common.Events.Handlers;

public sealed class ChatParticipantAddedEventHandler(
    INotificationRepository notificationRepository,
    IChatRepository chatRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : INotificationHandler<ChatParticipantAddedEventNotification>
{
    public async Task Handle(ChatParticipantAddedEventNotification notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        var chat = await chatRepository.GetByIdWithParticipantsAsync(e.ChatId, ct);
        
        if (chat is null)
        {
            return;
        }

        var addedBy = await userRepository.GetByIdAsync(chat.CreatorId, ct);
        
        if (addedBy is null)
        {
            return;
        }

        var payload = new AddedToGroupChatNotificationData(
            e.ChatId,
            chat.Name ?? "Group Chat",
            addedBy.Id,
            addedBy.UsernameNormalized);

        var serialized = JsonSerializer.Serialize(payload);
        
        var notif = Notification.Create(
            NotificationType.AddedToGroupChat, 
            serialized, 
            e.UserId, 
            addedBy.Id, 
            e.ChatId);
        
        await notificationRepository.AddAsync(notif, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}