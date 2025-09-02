using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Posts.Delete;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Result>
{
    private readonly ILogger<DeletePostCommandHandler> _logger;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePostCommandHandler(
        ILogger<DeletePostCommandHandler> logger, 
        IPostRepository postRepository, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeletePostCommand {@Command}.", request);

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
        {
            _logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }

        if (post.UserId != request.UserId)
        {
            _logger.LogWarning("User {UserId} doesn't own post {PostId}, access denied.", request.UserId, request.PostId);

            return Result.Failure("Access denied.", ErrorType.Forbidden);
        }

        try
        {
            await _postRepository.RemoveAsync(post.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Post {PostId} successfully deleted by user {UserId}.", post.Id, request.UserId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while deleting post {PostId} by user {UserId}.", 
                request.PostId, request.UserId);
            
            return Result.Failure("An internal error occurred.", ErrorType.ServerError);
        }
    }
}