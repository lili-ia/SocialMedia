using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Posts.ChangeActiveStatus;

public class ChangePostActiveStatusCommandHandler : IRequestHandler<ChangePostActiveStatusCommand, Result<Guid>>
{
    private readonly ILogger<ChangePostActiveStatusCommandHandler> _logger;
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePostActiveStatusCommandHandler(
        ILogger<ChangePostActiveStatusCommandHandler> logger, 
        IPostRepository postRepository, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ChangePostActiveStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ChangePostActiveStatusCommand {@Command}.", request);
        
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);

        if (post is null)
        {
            _logger.LogWarning("Post {PostId} not found.", request.PostId);
            
            return Result<Guid>.Failure("Post not found.", ErrorType.NotFound);
        }

        if (post.UserId != request.UserId)
        {
            _logger.LogWarning("User {UserId} doesn't own post {PostId}, access denied.", request.UserId, request.PostId);

            return Result<Guid>.Failure("Access denied.", ErrorType.Forbidden);
        }

        if (post.IsActive == request.ActiveStatus)
        {
            return Result<Guid>.Success(post.Id);
        }
        
        post.IsActive = request.ActiveStatus;
        post.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("User {UserId} successfully changed post {PostId} active status to {ActiveStatus}.", 
                request.UserId, request.PostId, request.ActiveStatus);

            return Result<Guid>.Success(post.Id);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while user {UserId} changing post {PostId} active status to {ActiveStatus}.", 
                request.UserId, request.PostId, request.ActiveStatus);
            
            return Result<Guid>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}