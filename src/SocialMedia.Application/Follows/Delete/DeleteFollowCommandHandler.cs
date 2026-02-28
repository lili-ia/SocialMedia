using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Follow;

namespace SocialMedia.Application.Follows.Delete;

public class DeleteFollowCommandHandler(
    IFollowRepository followRepository,
    ILogger<DeleteFollowCommandHandler> logger,
    ICacheService cache)
    : IRequestHandler<DeleteFollowCommand, Result>
{
    public async Task<Result> Handle(DeleteFollowCommand request, CancellationToken cancellationToken)
    {
        if (request.FollowerId == request.FolloweeId)
        {
            return Result.Failure("You can not unblock yourself.", ErrorType.Forbidden);
        }
        
        var affected = await followRepository.RemoveAsync(request.FollowerId, request.FolloweeId, cancellationToken);

        if (affected == 0)
        {
            logger.LogInformation("Follow relationship does not exist between {FollowerId} and {FolloweeId}.", 
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("Follow not found.", ErrorType.NotFound);
        }

        logger.LogInformation("Follow relationship deleted between {FollowerId} and {FolloweeId}.",
            request.FollowerId, request.FolloweeId);

        await cache.RemoveAsync($"feed:followees:user:{request.FollowerId}");
        await cache.RemoveAsync($"followers:user:{request.FolloweeId}");
        await cache.RemoveAsync($"followee:user:{request.FollowerId}");
        
        return Result.Success();
    }
}