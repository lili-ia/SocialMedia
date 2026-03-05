using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Chat;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Chats.Create;

public class CreateChatCommandHandler(
    IChatRepository chatRepository,
    IUserRepository userRepository,
    IBlockCacheService blockCacheService,
    IUnitOfWork unitOfWork,
    ILogger<CreateChatCommandHandler> logger)
    : IRequestHandler<CreateChatCommand, Result<ChatDto>>
{
    public async Task<Result<ChatDto>> Handle(CreateChatCommand request, CancellationToken ct)
    {
        var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.RequesterId, ct);

        if (!request.IsGroup)
        {
            var otherUserId = request.ParticipantIds[0];
            
            if (blockedIds.Contains(otherUserId))
            {
                return Result<ChatDto>.Failure("User not found.", ErrorType.NotFound);
            }

            var existing = await chatRepository.GetDirectChatBetweenAsync(request.RequesterId, otherUserId, ct);
            
            if (existing is not null)
            {
                return Result<ChatDto>.Failure("Chat already exists.", ErrorType.Conflict);
            }

            var otherUserExists = await userRepository.ExistsAsync(otherUserId, UserRole.User, ct);
            
            if (!otherUserExists)
            {
                return Result<ChatDto>.Failure("User not found.", ErrorType.NotFound);
            }

            var directChat = Chat.CreateDirect(request.RequesterId, otherUserId);
            await chatRepository.AddAsync(directChat, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Direct chat {ChatId} created between {A} and {B}.", directChat.Id, request.RequesterId, otherUserId);

            return Result<ChatDto>.Success(directChat.ToDto());
        }

        var anyBlocked = request.ParticipantIds.Any(id => blockedIds.Contains(id));

        if (anyBlocked)
        {
            return Result<ChatDto>.Failure("One or more participants not found.", ErrorType.NotFound);
        }

        var groupChat = Chat.CreateGroup(request.RequesterId, request.GroupName!, request.ParticipantIds);
        
        await chatRepository.AddAsync(groupChat, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Group chat {ChatId} created by {CreatorId} with {Count} participants.",
            groupChat.Id, request.RequesterId, request.ParticipantIds.Count);

        return Result<ChatDto>.Success(groupChat.ToDto());
    }
}