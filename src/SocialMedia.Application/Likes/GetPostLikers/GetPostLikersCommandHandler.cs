using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Likes.GetPostLikers;

public class GetPostLikersCommandHandler(
    ILogger<GetPostLikersCommandHandler> logger,
    IPostLikeRepository postLikeRepository,
    IPostRepository postRepository,
    IBlockRepository blockRepository,
    IValidator<GetPostLikersCommand> validator)
    : IRequestHandler<GetPostLikersCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetPostLikersCommand request, CancellationToken ct)
    {
        var postAuthorId = await postRepository.GetUserIdByPostIdAsync(request.PostId, ct);

        if (postAuthorId is null)
        {
            logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<IReadOnlyList<UserPreviewDto>>.Failure("Post not found.", ErrorType.NotFound);
        }

        var blockExists = await blockRepository
            .IsBlockedByEitherAsync(request.TargetUserId, postAuthorId.Value, ct);

        if (blockExists)
        {
            logger.LogWarning("There is a block between {TargetUserId} and {PostAuthorId}.", 
                request.TargetUserId, postAuthorId.Value);
                
            return Result<IReadOnlyList<UserPreviewDto>>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        var skip = (request.Page - 1) * request.PageSize;
        
        var postLikers = await postLikeRepository
            .GetNotBlockedPostLikersAsync(
                postId: request.PostId, 
                targetUserId: request.TargetUserId, 
                selector: PostLikeMapper.ProjectToUserPreviewDto, 
                skip: skip, 
                take: request.PageSize,
                ct);
        
        logger.LogInformation("Retrieved {Count} likes for post {PostId} for user {TargetUserId}.", 
            postLikers.Count, request.PostId, request.TargetUserId);
        
        return Result<IReadOnlyList<UserPreviewDto>>.Success(postLikers);
    }
}