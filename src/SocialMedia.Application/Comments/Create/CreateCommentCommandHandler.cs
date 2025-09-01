using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Comments.Create;

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<CommentDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCommentCommand> _validator;
    private readonly ILogger<CreateCommentCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IBlockRepository _blockRepository;

    public CreateCommentCommandHandler(
        IPostRepository postRepository, 
        IUnitOfWork unitOfWork, 
        IValidator<CreateCommentCommand> validator, 
        ILogger<CreateCommentCommandHandler> logger, 
        IUserRepository userRepository, 
        ICommentRepository commentRepository, 
        IBlockRepository blockRepository)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
        _userRepository = userRepository;
        _commentRepository = commentRepository;
        _blockRepository = blockRepository;
    }

    public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateCommentCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreateCommentCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<CommentDto>();
        }

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        
        if (post is null || (!post.IsActive && request.UserId != post.UserId))
        {
            _logger.LogInformation("Post {PostId} not found or not active.", request.PostId);
            
            return Result<CommentDto>.Failure("Post not found.", ErrorType.NotFound);
        }
        
        if (request.UserId != post.UserId)
        {
            var blockExists = await _blockRepository
                .IsBlockedByEitherAsync(post.UserId, request.UserId, cancellationToken);

            if (blockExists)
            {
                _logger.LogInformation("There is a block between {PostAuthorId} and {CommentAuthorId}.", 
                    post.UserId, request.UserId);
                
                return Result<CommentDto>.Failure("Post not found.", ErrorType.NotFound);
            }
        }
        
        var username = await _userRepository.GetUsernameByIdAsync(request.UserId, cancellationToken);

        if (username is null)
        {
            _logger.LogWarning("User {UserId} attempted to create a comment but has no username.", request.UserId);
            
            return Result<CommentDto>.Failure("Access denied.", ErrorType.Forbidden);
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            UserId = request.UserId,
            PostId = request.PostId,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await _commentRepository.AddAsync(comment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment {CommentId} successfully created by user {UserId}.",
                comment.Id, request.UserId);
            
            var commentDto = CommentMapper.ToDto(comment, username);
            
            return Result<CommentDto>.Success(commentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while creating a comment by user {UserId} to post {PostId}.", 
                request.UserId, request.PostId);
            
            return Result<CommentDto>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}