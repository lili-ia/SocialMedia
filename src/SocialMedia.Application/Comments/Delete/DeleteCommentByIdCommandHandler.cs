using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Comments.Delete;

public class DeleteCommentByIdCommandHandler : IRequestHandler<DeleteCommentByIdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCommentByIdCommandHandler> _logger;
    private readonly ICommentRepository _commentRepository;
    private readonly IBlockRepository _blockRepository;

    public DeleteCommentByIdCommandHandler(
        IUnitOfWork unitOfWork, 
        ILogger<DeleteCommentByIdCommandHandler> logger, 
        ICommentRepository commentRepository, 
        IBlockRepository blockRepository)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _commentRepository = commentRepository;
        _blockRepository = blockRepository;
    }

    public async Task<Result> Handle(DeleteCommentByIdCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteCommentByIdCommand {@Command}.", request);

        var comment = await _commentRepository.GetByIdWithPostAsync(request.CommentId, cancellationToken);
        
        if (comment is null)
        {
            _logger.LogWarning("Comment {CommentId} not found.", request.CommentId);
            
            return Result.Failure("Comment not found.", ErrorType.NotFound);
        }
        
        if (!comment.Post.IsActive)
        {
            _logger.LogInformation("Post {PostId} is not active.", comment.PostId);
            
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }
        
        var blockExists = await _blockRepository
            .IsBlockedByEitherAsync(comment.UserId, request.UserId, cancellationToken);

        if (blockExists)
        {
            _logger.LogInformation("There is a block between {PostAuthorId} and {CommentAuthorId}.", 
                comment.Post.UserId, request.UserId);
                
            return Result.Failure("Post not found.", ErrorType.NotFound);
        }

        if (request.UserId != comment.UserId && request.UserId != comment.Post.UserId)
        {
            _logger.LogInformation("User {UserId} can't delete comment {CommentId} they do not own.", 
                request.UserId, request.CommentId);
            
            return Result.Failure("Access denied.", ErrorType.Forbidden);
        }
        
        try
        {
            await _commentRepository.RemoveAsync(request.CommentId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment {CommentId} successfully deleted by user {UserId}.",
                request.CommentId, request.UserId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while deleting comment {CommentId} by user {UserId}.", 
                request.CommentId, request.UserId);
            
            return Result.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}