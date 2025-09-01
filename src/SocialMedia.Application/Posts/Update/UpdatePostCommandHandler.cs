using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Posts.Update;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result<PostDto>>
{
    private readonly ILogger<UpdatePostCommandHandler> _logger;
    private readonly IValidator<UpdatePostCommand> _validator;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdatePostCommandHandler(
        IValidator<UpdatePostCommand> validator, 
        ILogger<UpdatePostCommandHandler> logger, 
        IPostRepository postRepository, 
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _logger = logger;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<PostDto>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdatePostCommand {@Command}.", request);
        
        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdatePostCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<PostDto>();
        }

        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
        {
            _logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<PostDto>.Failure("Post not found.", ErrorType.NotFound);
        }

        if (post.UserId != request.UserId)
        {
            _logger.LogWarning("User {UserId} doesn't own post {PostId}, access denied.", request.UserId, request.PostId);

            return Result<PostDto>.Failure("Access denied.", ErrorType.Forbidden);
        }

        post.Text = request.Text;
        post.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Post {PostId} successfully updated by user {UserId}.", post.Id, request.UserId);

            var dto = await _postRepository.GetDetailsAsync(post.Id, PostMapper.ProjectToDto, cancellationToken);

            if (dto is not null)
            {
                return Result<PostDto>.Success(dto);
            }
            
            _logger.LogWarning("Post {PostId} details not found after update.", post.Id);
                
            return Result<PostDto>.Failure("Post details not found after update.", ErrorType.NotFound);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while updating post {PostId} by user {UserId}.", 
                request.PostId, request.UserId);
            
            return Result<PostDto>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}