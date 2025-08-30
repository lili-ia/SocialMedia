using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Like;

namespace SocialMedia.Application.Likes.Create;

public class CreatePostLikeCommandHandler : IRequestHandler<CreatePostLikeCommand, Result<PostLikeResponse>>
{
    private readonly ILogger<CreatePostLikeCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly IPostRepository _postRepository;
    private readonly IBlockRepository _blockRepository;

    public CreatePostLikeCommandHandler(
        ILogger<CreatePostLikeCommandHandler> logger, 
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

    public async Task<Result<PostLikeResponse>> Handle(CreatePostLikeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreatePostLikeCommand {@Command}.", request);
        
        var postAuthorId = await _postRepository.GetUserIdByPostId(request.PostId, cancellationToken);

        if (postAuthorId is null)
        {
            _logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<PostLikeResponse>.Failure("Post not found.", ErrorType.NotFound);
        }

        var blockExists = await _blockRepository
            .IsBlockedByEitherAsync(request.LikerId, postAuthorId.Value, cancellationToken);

        if (blockExists)
        {
            _logger.LogWarning("There is a block between {LikerId} and {PostAuthorId}.", 
                request.LikerId, postAuthorId.Value);
                
            return Result<PostLikeResponse>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        var alreadyLiked = await _postLikeRepository.ExistsAsync(request.LikerId, request.PostId, cancellationToken);
        
        if (alreadyLiked)
        {
            _logger.LogInformation("User {LikerId} already liked post {PostId}.", request.LikerId, request.PostId);
            
            return Result<PostLikeResponse>.Failure("You already liked this post.", ErrorType.Conflict);
        }

        var like = new PostLike
        {
            Id = Guid.NewGuid(),
            UserId = request.LikerId,
            PostId = request.PostId,
            LikedAt = DateTime.UtcNow
        };

        try
        {
            await _postLikeRepository.AddAsync(like, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {LikerId} successfully liked post {PostId} by user {PostAuthorId}.",
                request.LikerId, request.PostId, postAuthorId);

            var likeCount = await _postLikeRepository.GetLikeCount(request.PostId, cancellationToken);
            
            return Result<PostLikeResponse>.Success(new PostLikeResponse
            {
                IsLiked = true,
                LikeCount = likeCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while user {UserId} liking a post {PostId} by {PostAuthorId}.", 
                request.LikerId, request.PostId, postAuthorId);
            
            return Result<PostLikeResponse>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}