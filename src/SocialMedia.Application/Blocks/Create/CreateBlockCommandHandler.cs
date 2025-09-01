using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Blocks.Create;

public class CreateBlockCommandHandler : IRequestHandler<CreateBlockCommand, Result<Guid>>
{
    private readonly ILogger<CreateBlockCommandHandler> _logger;
    private readonly IBlockRepository _blockRepository;
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBlockCommandHandler(
        ILogger<CreateBlockCommandHandler> logger, 
        IBlockRepository blockRepository, 
        IFollowRepository followRepository, 
        IUnitOfWork unitOfWork, 
        IUserRepository userRepository)
    {
        _logger = logger;
        _blockRepository = blockRepository;
        _followRepository = followRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public async Task<Result<Guid>> Handle(CreateBlockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateBlockCommand {@Command}.", request);

        if (request.BlockedId == request.BlockerId)
        {
            return Result<Guid>.Failure("You can not block yourself.", ErrorType.Forbidden);
        }
        
        var blockedUserExists = await _userRepository.ExistsAsync(request.BlockedId, UserRole.User, cancellationToken);
        
        if (!blockedUserExists)
        {
            _logger.LogWarning("Blocked user {UserId} not found.", request.BlockedId);
            
            return Result<Guid>.Failure("User not found.", ErrorType.NotFound);
        }

        var blockExists = await _blockRepository.ExistsAsync(request.BlockerId, request.BlockedId, cancellationToken);

        if (blockExists)
        {
            _logger.LogWarning("User {BlockerId} already blocked user {BlockedId}.", request.BlockerId, request.BlockedId);
            
            return Result<Guid>.Failure("Block already exists.", ErrorType.Conflict);
        }

        var block = new Block
        {
            Id = Guid.NewGuid(),
            BlockerId = request.BlockerId,
            BlockedId = request.BlockedId,
            BlockedAt = DateTime.UtcNow
        };

        try
        {
            await _followRepository.RemoveMutualAsync(request.BlockerId, request.BlockedId, cancellationToken);
            await _blockRepository.AddAsync(block, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {BlockerId} successfully blocked user {BlockedId}.",
                request.BlockerId, request.BlockedId);

            return Result<Guid>.Success(block.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while user {BlockerId} trying to block user {BlockedId}.", 
                request.BlockerId, request.BlockedId);
            
            return Result<Guid>.Failure("An internal error occurred.", ErrorType.ServerError);
        }
    }
}