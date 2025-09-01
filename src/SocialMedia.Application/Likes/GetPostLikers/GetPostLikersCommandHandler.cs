using System.Linq.Expressions;
using Domain.Entities;
using FluentValidation;
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
    private readonly IValidator<GetPostLikersCommand> _validator;

    public GetPostLikersCommandHandler(
        ILogger<GetPostLikersCommandHandler> logger, 
        IPostLikeRepository postLikeRepository, 
        IPostRepository postRepository, 
        IBlockRepository blockRepository, 
        IValidator<GetPostLikersCommand> validator)
    {
        _logger = logger;
        _postLikeRepository = postLikeRepository;
        _postRepository = postRepository;
        _blockRepository = blockRepository;
        _validator = validator;
    }

    public async Task<Result<IReadOnlyList<UserPreviewDto>>> Handle(GetPostLikersCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetPostLikersCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreatePostCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<UserPreviewDto>>();
        }
        
        var postAuthorId = await _postRepository.GetUserIdByPostIdAsync(request.PostId, cancellationToken);

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

        var skip = (request.Page - 1) * request.PageSize;
        
        var postLikers = await _postLikeRepository
            .GetPostLikersAsync(
                postId: request.PostId, 
                filter: eitherBlockedFilter, 
                selector: PostLikeMapper.ProjectToUserPreviewDto, 
                skip: skip, 
                take: request.PageSize,
                cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} likes for post {PostId} for user {TargetUserId}.", 
            postLikers.Count, request.PostId, request.TargetUserId);
        
        return Result<IReadOnlyList<UserPreviewDto>>.Success(postLikers);
    }
}