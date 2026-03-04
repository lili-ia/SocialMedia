using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Users.Deactivate;

public class DeactivateUserCommandHandler(
    ILogger<DeactivateUserCommandHandler> logger,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateUserCommand, Result>
{
    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct, tracking: true);

        if (user is null)
        {
            logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result.Failure("User not found.", ErrorType.NotFound);
        }
        
        user.ChangeStatus(UserStatus.Deactivated, "User voluntarily deactivated their account.");
        
        await unitOfWork.SaveChangesAsync(ct);
        
        logger.LogInformation("User {UserId} successfully deactivated their profile.", request.UserId);
        
        return Result.Success();
    }
}