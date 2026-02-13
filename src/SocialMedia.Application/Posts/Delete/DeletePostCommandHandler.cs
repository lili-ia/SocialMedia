using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Posts.Delete;

public class DeletePostCommandHandler(
    ILogger<DeletePostCommandHandler> logger,
    IPostRepository postRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePostCommand, Result>
{
    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);

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

        try
        {
            await postRepository.RemoveAsync(post.Id, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            logger.LogInformation("Post {PostId} successfully deleted by user {UserId}.", post.Id, request.UserId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while deleting post {PostId} by user {UserId}.", 
                request.PostId, request.UserId);
            
            return Result.InternalError();
        }
    }
}