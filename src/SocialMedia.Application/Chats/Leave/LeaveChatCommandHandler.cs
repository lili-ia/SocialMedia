using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Chats.AddParticipant;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Chats.Leave;

public class LeaveChatCommandHandler(IChatRepository chatRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<AddParticipantCommandHandler> logger)
    : IRequestHandler<LeaveChatCommand, Result>
{
    public async Task<Result> Handle(LeaveChatCommand request, CancellationToken ct)
    {
        var chat = await chatRepository.GetByIdWithParticipantsAsync(request.ChatId, ct);

        if (chat is null)
        {
            return Result.Failure("Chat not found.", ErrorType.NotFound);
        }

        var userExists = await userRepository.ExistsAsync(request.UserId, UserRole.User, ct);

        if (!userExists)
        {
            return Result.Failure("User not found.", ErrorType.NotFound);
        }
        
        chat.Leave(request.UserId);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User {UserId} left chat {ChatId}.", request.UserId, request.ChatId);

        return Result.Success();
    }
}