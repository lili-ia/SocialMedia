using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Comments.GetById;

public class GetCommentByIdCommandHandler : IRequestHandler<GetCommentByIdCommand, Result<CommentDto>>
{
    private readonly ILogger<GetCommentByIdCommandHandler> _logger;
    private readonly ICommentRepository _commentRepository;
    private readonly IBlockRepository _blockRepository;

    public GetCommentByIdCommandHandler(
        ILogger<GetCommentByIdCommandHandler> logger,
        ICommentRepository commentRepository,
        IBlockRepository blockRepository)
    {
        _logger = logger;
        _commentRepository = commentRepository;
        _blockRepository = blockRepository;
    }

    public async Task<Result<CommentDto>> Handle(GetCommentByIdCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCommentByIdCommand {@Command}.", request);

        var comment = await _commentRepository.GetByIdWithPostAsync(request.CommentId, cancellationToken);

        if (comment is null)
        {
            _logger.LogWarning("Comment {CommentId} not found.", request.CommentId);

            return Result<CommentDto>.Failure("Comment not found.", ErrorType.NotFound);
        }
        
        var blockExists = await _blockRepository
            .IsBlockedByEitherAsync(comment.UserId, request.TargetUserId, cancellationToken);

        if (blockExists)
        {
            _logger.LogInformation("There is a block between {PostAuthorId} and {CommentAuthorId}.",
                comment.Post.UserId, request.TargetUserId);

            return Result<CommentDto>.Failure("Post not found.", ErrorType.NotFound);
        }

        var commentDto = comment.ToDto();
        
        return Result<CommentDto>.Success(commentDto);
    }
}