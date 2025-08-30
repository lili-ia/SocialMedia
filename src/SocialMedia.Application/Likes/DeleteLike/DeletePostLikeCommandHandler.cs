using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Likes.DeleteLike;

public class DeletePostLikeCommandHandler : IRequestHandler<DeletePostLikeCommand, Result>
{
    private readonly ILogger<DeletePostLikeCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly IPostRepository _postRepository;
    private readonly IBlockRepository _blockRepository;

    public DeletePostLikeCommandHandler(
        ILogger<DeletePostLikeCommandHandler> logger, 
        IUnitOfWork unitOfWork, 
        IPostLikeRepository postLikeRepository, 
        IPostRepository postRepository, 
        IBlockRepository blockRepository)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _postLikeRepository = postLikeRepository;
        _postRepository = postRepository;
        _blockRepository = blockRepository;
    }

    public async Task<Result> Handle(DeletePostLikeCommand request, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Handling DeletePostLikeCommand {@Command}.", request);
         
        var postAuthorId = await _postRepository.GetUserIdByPostId(request.PostId, cancellationToken);

        if (postAuthorId is null)
        {
            _logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }

        var blockExists = await _blockRepository
            .IsBlockedByEitherAsync(request.LikerId, postAuthorId.Value, cancellationToken);

        if (blockExists)
        {
            _logger.LogWarning("There is a block between {LikerId} and {PostAuthorId}.", 
                request.LikerId, postAuthorId.Value);
                
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }
        
        var alreadyLiked = await _postLikeRepository.ExistsAsync(request.LikerId, request.PostId, cancellationToken);
        
        if (!alreadyLiked)
        {
            _logger.LogInformation("User {LikerId} haven't liked post {PostId}.", request.LikerId, request.PostId);
            
            return Result.Failure("Like not found.", ErrorType.NotFound);
        }

        try
        {
            await _postLikeRepository.RemoveAsync(request.LikerId, request.PostId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {LikerId} successfully unliked post {PostId} by user {PostAuthorId}.",
                request.LikerId, request.PostId, postAuthorId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while user {UserId} unliking a post {PostId} by {PostAuthorId}.", 
                request.LikerId, request.PostId, postAuthorId);
            
            return Result.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}