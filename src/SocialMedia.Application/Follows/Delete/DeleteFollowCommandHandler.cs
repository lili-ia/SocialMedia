using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Follow;

namespace SocialMedia.Application.Follows.Delete;

public class DeleteFollowCommandHandler : IRequestHandler<DeleteFollowCommand, Result<FollowResponse>>
{
    private readonly IFollowRepository _followRepository;
    private readonly ILogger<DeleteFollowCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteFollowCommand> _validator;

    public DeleteFollowCommandHandler(
        IFollowRepository followRepository, 
        ILogger<DeleteFollowCommandHandler> logger, 
        IUnitOfWork unitOfWork, 
        IValidator<DeleteFollowCommand> validator)
    {
        _followRepository = followRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<FollowResponse>> Handle(DeleteFollowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteFollowCommand {@Command}.", request);
        
        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for DeleteFollowCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<FollowResponse>();
        }
        
        var followExists = await _followRepository.ExistsAsync(request.FollowerId, request.FolloweeId, cancellationToken);

        if (!followExists)
        {
            _logger.LogInformation("Follow relationship does not exist between {FollowerId} and {FolloweeId}.", 
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("Follow not found.", ErrorType.NotFound);
        }
        
        try
        {
            await _followRepository.RemoveAsync(request.FollowerId, request.FolloweeId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Follow relationship deleted between {FollowerId} and {FolloweeId}.",
                request.FollowerId, request.FolloweeId);

            var followerCount = await _followRepository
                .GetActiveFollowerCountForUserIdAsync(request.FolloweeId, cancellationToken);

            return Result<FollowResponse>.Success(new FollowResponse
            {
                IsFollowed = false,
                FolloweeFollowerCount = followerCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting follow between {FollowerId} and {FolloweeId}.",
                request.FollowerId, request.FolloweeId);

            return Result<FollowResponse>.Failure("An internal error occurred.", ErrorType.ServerError);
        }
    }
}