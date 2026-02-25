using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Blocks.Delete;

public class DeleteBlockCommandHandler(
    ILogger<DeleteBlockCommandHandler> logger,
    IBlockRepository blockRepository)
    : IRequestHandler<DeleteBlockCommand, Result<MessageResponse>>
{
    public async Task<Result<MessageResponse>> Handle(DeleteBlockCommand request, CancellationToken cancellationToken)
    {
        if (request.BlockedId == request.BlockerId)
        {
            return Result<MessageResponse>.Failure("You can not unblock yourself.", ErrorType.Forbidden);
        }
        
        var affected = await blockRepository.RemoveAsync(request.BlockerId, request.BlockedId, cancellationToken);

        if (affected == 0)
        {
            logger.LogWarning("Block between user {BlockerId} and user {BlockedId} doesn't exist.",
                request.BlockerId, request.BlockedId);
            
            return Result<MessageResponse>.Failure("Block not found.", ErrorType.NotFound);
        }

        logger.LogInformation("User {BlockerId} successfully unblocked user {BlockedId}.",
            request.BlockerId, request.BlockedId);

        return Result<MessageResponse>.Success(new MessageResponse("You successfully unblocked user."));
    }
}