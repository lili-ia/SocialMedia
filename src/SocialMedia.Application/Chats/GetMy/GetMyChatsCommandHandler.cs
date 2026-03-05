using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Chat;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Chats.GetMy;

public class GetMyChatsCommandHandler(
    IChatRepository chatRepository,
    IFileStorageService storageService,
    ILogger<GetMyChatsCommandHandler> logger)
    : IRequestHandler<GetMyChatsCommand, Result<IReadOnlyList<ChatDto>>>
{
    public async Task<Result<IReadOnlyList<ChatDto>>> Handle(GetMyChatsCommand request, CancellationToken ct)
    {
        var chats = await chatRepository.GetChatsForUserAsync(
            request.UserId,
            ChatMapper.ProjectToChatDto,
            ct);

        foreach (var chat in chats)
        {
            foreach (var p in chat.Participants)
            {
                if (!string.IsNullOrEmpty(p.ThumbnailProfilePicUrl))
                {
                    p.ThumbnailProfilePicUrl = storageService.GetPresignedUrl(p.ThumbnailProfilePicUrl);
                }
            }

            if (chat.LastMessage is not null)
            {
                foreach (var attachment in chat.LastMessage.Attachments)
                {
                    attachment.Url = storageService.GetPresignedUrl(attachment.Url);
                }
            }
        }

        logger.LogInformation("Retrieved {Count} chats for user {UserId}.", chats.Count, request.UserId);

        return Result<IReadOnlyList<ChatDto>>.Success(chats);
    }
}