using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.GetById;

public class GetPostByIdCommandHandler : IRequestHandler<GetPostByIdCommand, Result<PostDto>>
{
    private readonly ILogger<GetPostByIdCommandHandler> _logger;
    private readonly IBlockRepository _blockRepository;
    private readonly IPostRepository _postRepository;
    private readonly IPostLikeRepository _postLikeRepository;

    public GetPostByIdCommandHandler(
        ILogger<GetPostByIdCommandHandler> logger, 
        IBlockRepository blockRepository, 
        IPostRepository postRepository, 
        IPostLikeRepository postLikeRepository)
    {
        _logger = logger;
        _blockRepository = blockRepository;
        _postRepository = postRepository;
        _postLikeRepository = postLikeRepository;
    }
    
    public async Task<Result<PostDto>> Handle(GetPostByIdCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetPostByIdCommand {@Command}.", request);

        var post = await _postRepository.GetDetailsAsync(request.PostId, PostMapper.ToDto, cancellationToken);

        if (post is null || (!post.IsActive && post.UserId != request.TargetUserId))
        {
            _logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<PostDto>.Failure("Post not found.", ErrorType.NotFound);
        }

        if (request.TargetUserId.HasValue)
        {
            post.IsLiked = await _postLikeRepository.IsLikedByUser(
                request.PostId, request.TargetUserId.Value, cancellationToken);
        }
        else
        {
            post.IsLiked = false;
        }
        
        if (request.TargetUserId is null)
        {
            _logger.LogInformation("Successfully retrieved post {PostId} details for user {UserId}.", 
                request.PostId, request.TargetUserId?.ToString() ?? "Anonymous");
            
            return Result<PostDto>.Success(post);
        }
        
        var blockExists = await _blockRepository
            .IsBlockedByEitherAsync(post.UserId, request.TargetUserId.Value, cancellationToken);

        if (!blockExists)
        {
            _logger.LogInformation("Successfully retrieved post {PostId} details for user {UserId}.", 
                request.PostId, request.TargetUserId);
            
            return Result<PostDto>.Success(post);
        }
        
        _logger.LogInformation("There is a block between {AuthorId} and {TargetUserId}.", 
            post.UserId, request.TargetUserId.Value);
                
        return Result<PostDto>.Failure("Post not found.", ErrorType.NotFound);
    }
}