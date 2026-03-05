using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using SocialMedia.Application.DTOs.Chat;

namespace SocialMedia.Application.Mappers;

public static class MessageMapper
{
    public static readonly Expression<Func<Message, MessageDto>> ProjectToMessageDto = m => new MessageDto
    {
        Id = m.Id,
        ChatId = m.ChatId,
        SenderId = m.SenderId,
        SenderUsername = m.Sender.UsernameNormalized,
        Text = m.Status == MessageStatus.Deleted ? null : m.Content,
        Status = m.Status,
        ParentMessageId = m.ParentMessageId,
        Attachments = m.Attachments.Select(a => 
                a.ToMessageAttachmentDto()).ToList(),
        CreatedAt = m.CreatedAt
    };

    public static MessageDto ToDto(this Message message, User sender) => new MessageDto
    {
        Id = message.Id,
        ChatId = message.ChatId,
        SenderId = message.SenderId,
        SenderUsername = sender.UsernameNormalized,
        SenderThumbnailProfilePicStorageKey = sender.CurrentProfilePic.ThumbnailStorageKey,
        Text = message.Content,
        Status = message.Status,
        ParentMessageId = message.ParentMessageId,
        Attachments = message.Attachments.Select(a => 
            a.ToMessageAttachmentDto()).ToList(),
        CreatedAt = message.CreatedAt
    };

    private static MessageAttachmentDto ToMessageAttachmentDto(this MessageAttachment attachment) => new MessageAttachmentDto
    {
        Id = attachment.Id,
        FileName = attachment.FileName,
        ContentType = attachment.ContentType,
        FileSizeBytes = attachment.FileSizeBytes,
        StorageKey = attachment.StorageKey
    };
}