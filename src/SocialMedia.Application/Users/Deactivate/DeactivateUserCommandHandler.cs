using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Users.Deactivate;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly ILogger<DeactivateUserCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserCommandHandler(
        ILogger<DeactivateUserCommandHandler> logger, 
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteUserCommand {@Command}.", request);
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result.Failure("User not found.", ErrorType.NotFound);
        }
        
        user.Status = UserStatus.Deactivated;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("User {UserId} successfully deactivated their profile.", request.UserId);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while deactivating user {UserId} profile.", request.UserId);

            return Result.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}