using System.Linq.Expressions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Comments.GetAllForPost;

public class GetAllCommentsForPostCommandHandler : IRequestHandler<GetAllCommentsForPostCommand, Result<IReadOnlyList<CommentDto>>>
{
    private readonly ILogger<GetAllCommentsForPostCommandHandler> _logger;
    private readonly ICommentRepository _commentRepository;
    private readonly IValidator<GetAllCommentsForPostCommand> _validator;
    private readonly IPostRepository _postRepository;
    private readonly IBlockRepository _blockRepository;
    
    public GetAllCommentsForPostCommandHandler(
        ILogger<GetAllCommentsForPostCommandHandler> logger, 
        ICommentRepository commentRepository, 
        IValidator<GetAllCommentsForPostCommand> validator, 
        IPostRepository postRepository, IBlockRepository blockRepository)
    {
        _logger = logger;
        _commentRepository = commentRepository;
        _validator = validator;
        _postRepository = postRepository;
        _blockRepository = blockRepository;
    }

    public async Task<Result<IReadOnlyList<CommentDto>>> Handle(GetAllCommentsForPostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllCommentsForPostCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for GetAllCommentsForPostCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<IReadOnlyList<CommentDto>>();
        }

        var postStatus = await _postRepository.GetStatusAsync(request.PostId, cancellationToken);

        if (postStatus is null || !postStatus.Value.IsActive)
        {
            _logger.LogWarning("Post {PostId} not found or not active.", request.PostId);
            
            return Result<IReadOnlyList<CommentDto>>.Failure("Post not found.", ErrorType.NotFound);
        }

        if (request.TargetUserId != postStatus.Value.AuthorId)
        {
            var blockExists = await _blockRepository
                .IsBlockedByEitherAsync(postStatus.Value.AuthorId, request.TargetUserId, cancellationToken);

            if (blockExists)
            {
                _logger.LogInformation("There is a block between {PostAuthorId} and {TargetUserId}.", 
                    postStatus.Value.AuthorId, request.TargetUserId);
                
                return Result<IReadOnlyList<CommentDto>>.Failure("Post not found.", ErrorType.NotFound);
            }
        }
        
        Expression<Func<Comment, bool>> eitherBlockedFilter = comment => !comment.User.BlockedUsers
            .Any(b =>
                (b.BlockerId == request.TargetUserId && b.BlockedId == comment.UserId) ||
                (b.BlockerId == comment.UserId && b.BlockedId == request.TargetUserId));

        var skip = (request.Page - 1) * request.PageSize;
        
        var comments = await _commentRepository
            .GetAllByPostIdAsync(
                postId: request.PostId, 
                predicate: eitherBlockedFilter, 
                selector: CommentMapper.ProjectToDto, 
                skip: skip,
                take: request.PageSize,
                cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} comments to post {PostId} for user {TargetUserId}.", 
            comments.Count, request.PostId, request.TargetUserId);
        
        return Result<IReadOnlyList<CommentDto>>.Success(comments);
    }
}