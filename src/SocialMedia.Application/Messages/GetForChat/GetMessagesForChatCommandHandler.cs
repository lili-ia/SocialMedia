using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Chat;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Messages.GetForChat;

public class GetMessagesForChatCommandHandler(
    IChatRepository chatRepository,
    IMessageRepository messageRepository,
    IFileStorageService storageService,
    ILogger<GetMessagesForChatCommandHandler> logger)
    : IRequestHandler<GetMessagesForChatCommand, Result<IReadOnlyList<MessageDto>>>
{
    public async Task<Result<IReadOnlyList<MessageDto>>> Handle(GetMessagesForChatCommand request, CancellationToken ct)
    {
        var isParticipant = await chatRepository.IsParticipantAsync(request.ChatId, request.RequesterId, ct);

        if (!isParticipant)
        {
            return Result<IReadOnlyList<MessageDto>>.Failure("Chat not found.", ErrorType.NotFound);
        }

        var skip = (request.Page - 1) * request.PageSize;

        var messages = await messageRepository.GetMessagesForChatAsync(
            request.ChatId,
            MessageMapper.ProjectToMessageDto,
            skip,
            request.PageSize,
            ct);

        foreach (var message in messages)
        {
            foreach (var attachment in message.Attachments)
            {
                attachment.Url = storageService.GetPresignedUrl(attachment.StorageKey);
            }

            if (!string.IsNullOrEmpty(message.SenderThumbnailProfilePicStorageKey))
            {
                message.SenderThumbnailProfilePicUrl = storageService.GetPresignedUrl(message.SenderThumbnailProfilePicStorageKey);
            }
        }

        logger.LogInformation("Retrieved {Count} messages for chat {ChatId}.", messages.Count, request.ChatId);

        return Result<IReadOnlyList<MessageDto>>.Success(messages);
    }
}