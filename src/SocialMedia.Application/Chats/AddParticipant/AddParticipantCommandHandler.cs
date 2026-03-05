using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Chats.AddParticipant;

public class AddParticipantCommandHandler(
    IChatRepository chatRepository,
    IUserRepository userRepository,
    IBlockCacheService blockCacheService,
    IUnitOfWork unitOfWork,
    ILogger<AddParticipantCommandHandler> logger)
    : IRequestHandler<AddParticipantCommand, Result>
{
    public async Task<Result> Handle(AddParticipantCommand request, CancellationToken ct)
    {
        var chat = await chatRepository.GetByIdWithParticipantsAsync(request.ChatId, ct);

        if (chat is null || !chat.IsParticipant(request.RequesterId))
        {
            return Result.Failure("Chat not found.", ErrorType.NotFound);
        }

        var blockedIds = await blockCacheService.GetBlockedAndBlockerIdsAsync(request.RequesterId, ct);

        if (blockedIds.Contains(request.NewUserId))
        {
            return Result.Failure("User not found.", ErrorType.NotFound);
        }

        var userExists = await userRepository.ExistsAsync(request.NewUserId, UserRole.User, ct);

        if (!userExists)
        {
            return Result.Failure("User not found.", ErrorType.NotFound);
        }

        chat.AddParticipant(request.RequesterId, request.NewUserId);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User {NewUserId} added to chat {ChatId} by {RequesterId}.",
            request.NewUserId, request.ChatId, request.RequesterId);

        return Result.Success();
    }
}