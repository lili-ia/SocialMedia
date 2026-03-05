using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using SocialMedia.Application.DTOs.Chat;

namespace SocialMedia.Application.Mappers;

public static class ChatMapper
{
    public static Expression<Func<Chat, ChatDto>> ProjectToChatDto => c => new ChatDto
    {
        Id = c.Id,
        Type = c.Type,
        Name = c.Name,
        Participants = c.Participants.Select(p => p.ToChatParticipantDto()).ToList(),
        LastMessage = new MessageDto
        {
            Id = c.LastMessage.Id,
            ChatId = c.LastMessage.ChatId,
            SenderId = c.LastMessage.SenderId,
            SenderUsername = c.LastMessage.Sender.UsernameNormalized,
            SenderThumbnailProfilePicStorageKey = c.LastMessage.Sender.CurrentProfilePic.ThumbnailStorageKey,
            Text = c.LastMessage.Content,
            Status = c.LastMessage.Status,
            ParentMessageId = c.LastMessage.ParentMessageId,
            Attachments = c.LastMessage.Attachments.Select(a => new MessageAttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = ContentType.Image,
                FileSizeBytes = a.FileSizeBytes,
                StorageKey = a.StorageKey
            }).ToList(),
            CreatedAt = c.LastMessage.CreatedAt
        },
        UnreadCount = 0 // todo
    };
    
    public static ChatDto ToDto(this Chat chat) => new ChatDto
    {
        Id = chat.Id,
        Type = chat.Type,
        Name = chat.Name,
        Participants = chat.Participants.Select(p => p.ToChatParticipantDto()).ToList(),
        LastMessage = chat.LastMessage?.ToDto(chat.LastMessage.Sender),
        UnreadCount = 0 // todo
    };

    private static ChatParticipantDto ToChatParticipantDto(this ChatParticipant participant) =>
        new ChatParticipantDto
        {
            UserId = participant.UserId,
            Username = participant.User.UsernameNormalized,
            ThumbnailStorageKey = participant.User.CurrentProfilePic.ThumbnailStorageKey,
            IsAdmin = participant.IsAdmin
        };
}