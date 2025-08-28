using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Blocks.Delete;

public class DeleteBlockCommandHandler : IRequestHandler<DeleteBlockCommand, Result>
{
    private readonly ILogger<DeleteBlockCommandHandler> _logger;
    private readonly IBlockRepository _blockRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBlockCommandHandler(
        ILogger<DeleteBlockCommandHandler> logger, 
        IBlockRepository blockRepository, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _blockRepository = blockRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBlockCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteBlockCommand {@Command}.", request);

        if (request.BlockedId == request.BlockerId)
        {
            return Result.Failure("You can not unblock yourself.", ErrorType.Forbidden);
        }
        
        var blockedUserExists = await _userRepository.ExistsAsync(request.BlockedId, UserRole.User, cancellationToken);
        
        if (!blockedUserExists)
        {
            _logger.LogWarning("Blocked user {UserId} not found.", request.BlockedId);
            
            return Result.Failure("User not found.", ErrorType.NotFound);
        }

        var blockExists = await _blockRepository.ExistsAsync(request.BlockerId, request.BlockedId, cancellationToken);

        if (!blockExists)
        {
            _logger.LogWarning("Block between user {BlockerId} and user {BlockedId} doesn't exist.",
                request.BlockerId, request.BlockedId);
            
            return Result.Failure("Block not found.", ErrorType.NotFound);
        }

        try
        {
            await _blockRepository.RemoveAsync(request.BlockerId, request.BlockedId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {BlockerId} successfully unblocked user {BlockedId}.",
                request.BlockerId, request.BlockedId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while user {BlockerId} trying to unblock user {BlockedId}.", 
                request.BlockerId, request.BlockedId);
            
            return Result.Failure("An internal error occurred.", ErrorType.ServerError);
        }
    }
}