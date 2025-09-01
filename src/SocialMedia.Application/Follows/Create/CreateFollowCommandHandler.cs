using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Follow;

namespace SocialMedia.Application.Follows.Create;

public class CreateFollowCommandHandler : IRequestHandler<CreateFollowCommand, Result<FollowResponse>>
{
    private readonly IFollowRepository _followRepository;
    private readonly IBlockRepository _blockRepository;
    private readonly ILogger<CreateFollowCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateFollowCommand> _validator;
    
    public CreateFollowCommandHandler(
        IFollowRepository followRepository, 
        IBlockRepository blockRepository, 
        ILogger<CreateFollowCommandHandler> logger,
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork, 
        IValidator<CreateFollowCommand> validator)
    {
        _followRepository = followRepository;
        _blockRepository = blockRepository;
        _logger = logger;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<FollowResponse>> Handle(CreateFollowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateFollowCommand {@Command}.", request);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for CreateFollowCommand: {Errors}", validationResult.Errors);
            
            return validationResult.ToFailureResult<FollowResponse>();
        }

        var blockExists = await _blockRepository
            .IsBlockedByEitherAsync(request.FollowerId, request.FolloweeId, cancellationToken);
            
        if (blockExists)
        {
            _logger.LogInformation("There is a block between {FollowerId} and {FolloweeId}.",
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("Followee not found.", ErrorType.NotFound);
        }
        
        var followExists = await _followRepository.ExistsAsync(request.FollowerId, request.FolloweeId, cancellationToken);

        if (followExists)
        {
            _logger.LogInformation("User {FollowerId} already follows user {FolloweeId}.", 
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("You already follow this user.", ErrorType.Conflict);
        }

        var followeeExists = await _userRepository.ExistsAsync(request.FolloweeId, UserRole.User, cancellationToken);

        if (!followeeExists)
        {
            _logger.LogWarning("User {FolloweeId} not found.", request.FolloweeId);
            
            return Result<FollowResponse>.Failure("Followee not found.", ErrorType.NotFound);
        }

        var follow = new Follow
        {
            FollowerId = request.FollowerId,
            FolloweeId = request.FolloweeId,
            FollowedAt = DateTime.UtcNow
        };

        try
        {
            await _followRepository.AddAsync(follow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("User {FollowerId} successfully followed user {FolloweeId}.", 
                request.FollowerId, request.FolloweeId);

            var followerCount = await _followRepository
                .GetActiveFollowerCountForUserIdAsync(request.FolloweeId, cancellationToken);
            
            return Result<FollowResponse>.Success(new FollowResponse
            {
                IsFollowed = true,
                FolloweeFollowerCount = followerCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while user {FollowerId} following user {FolloweeId}.", 
                request.FollowerId, request.FolloweeId);
            
            return Result<FollowResponse>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}