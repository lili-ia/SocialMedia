using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Follow;

namespace SocialMedia.Application.Follows.Delete;

public class DeleteFollowCommandHandler(
    IFollowRepository followRepository,
    ILogger<DeleteFollowCommandHandler> logger,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteFollowCommand, Result>
{
    public async Task<Result> Handle(DeleteFollowCommand request, CancellationToken ct)
    {
        if (request.FollowerId == request.FolloweeId)
        {
            return Result.Failure("You can not unblock yourself.", ErrorType.Forbidden);
        }

        var follow = await followRepository.GetByFollowerAndFolloweeIdsAsync(request.FollowerId, request.FolloweeId, ct);

        if (follow is null)
        {
            logger.LogInformation("Follow relationship does not exist between {FollowerId} and {FolloweeId}.", 
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("Follow not found.", ErrorType.NotFound);
        }
        
        follow.SoftDelete();
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Follow relationship deleted between {FollowerId} and {FolloweeId}.",
            request.FollowerId, request.FolloweeId);
        
        return Result.Success();
    }
}