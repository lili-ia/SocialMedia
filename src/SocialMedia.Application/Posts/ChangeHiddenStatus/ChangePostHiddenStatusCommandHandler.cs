using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Posts.ChangeHiddenStatus;

public class ChangePostHiddenStatusCommandHandler(
    ILogger<ChangePostHiddenStatusCommandHandler> logger,
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    ICacheService cache)
    : IRequestHandler<ChangePostHiddenStatusCommand, Result>
{
    public async Task<Result> Handle(ChangePostHiddenStatusCommand request, CancellationToken ct)
    {
        var post = await postRepository.GetByIdAsync(request.PostId, ct);

        if (post is null)
        {
            logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }
        
        if (post.UserId != request.UserId)
        {
            logger.LogWarning("User {UserId} doesn't own post {PostId}, access denied.", request.UserId, request.PostId);

            return Result.Failure("Access denied.", ErrorType.Forbidden);
        }

        if (post.IsHidden == request.MustBeHidden)
        {
            var status = post.IsHidden ? "hidden" : "active";
    
            return Result.Failure($"Post is already {status}.", ErrorType.Conflict);
        }

        post.SetHiddenStatus(request.MustBeHidden);
        
        await unitOfWork.SaveChangesAsync(ct);
        await cache.RemoveByPrefixAsync("feed:popular");
        await cache.RemoveAsync($"posts:{request.PostId}");
        
        return Result.Success();
    }
}