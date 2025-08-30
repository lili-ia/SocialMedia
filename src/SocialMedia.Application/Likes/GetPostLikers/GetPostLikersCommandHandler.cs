using System.Linq.Expressions;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Likes.GetPostLikers;

public class GetPostLikersCommandHandler : IRequestHandler<GetPostLikersCommand, Result<IReadOnlyList<UserPreviewDto>>>
{
    private readonly ILogger<GetPostLikersCommandHandler> _logger;
    private readonly IPostLikeRepository _postLikeRepository;
    private readonly IPostRepository _postRepository;
    private readonly IBlockRepository _blockRepository;

    public GetPostLikersCommandHandler(
        ILogger<GetPostLikersCommandHandler> logger, 
        IPostLikeRepository postLikeRepository, 
        IPostRepository postRepository, 
        IBlockRepository blockRepository)
    {
        _logger = logger;
        _postLikeRepository = postLikeRepository;
        _postRepository = postRepository;
        _blockRepository = blockRepository;
    }

    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetPostLikersCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetPostLikersCommand {@Command}.", request);

        var postAuthorId = await _postRepository.GetUserIdByPostId(request.PostId, cancellationToken);

        if (postAuthorId is null)
        {
            _logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<IReadOnlyList<UserPreviewDto>>.Failure("Post not found.", ErrorType.NotFound);
        }

        var blockExists = await _blockRepository
            .IsBlockedByEitherAsync(request.TargetUserId, postAuthorId.Value, cancellationToken);

        if (blockExists)
        {
            _logger.LogWarning("There is a block between {TargetUserId} and {PostAuthorId}.", 
                request.TargetUserId, postAuthorId.Value);
                
            return Result<IReadOnlyList<UserPreviewDto>>.Failure("Post not found.", ErrorType.NotFound);
        }

        Expression<Func<PostLike, bool>> eitherBlockedFilter = like => !like.User.BlockedUsers
            .Any(b =>
                (b.BlockerId == request.TargetUserId && b.BlockedId == like.UserId) ||
                (b.BlockerId == like.UserId && b.BlockedId == request.TargetUserId));

        var postLikers = await _postLikeRepository
            .GetPostLikers(request.PostId, eitherBlockedFilter, PostLikeMapper.ToUserPreviewDto,
                cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} likes for post {PostId} for user {TargetUserId}.", 
            postLikers.Count, request.PostId, request.TargetUserId);
        
        return Result<IReadOnlyList<UserPreviewDto>>.Success(postLikers);
    }
}