using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Authentication.ResetPassword;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IHashService hashService,
    ITokenRepository tokenRepository,
    IUnitOfWork unitOfWork,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, Result<MessageResponse>>
{
    public async Task<Result<MessageResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLower();

        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            return Result<MessageResponse>.Failure("Invalid request.", ErrorType.BadRequest);
        }

        var hashedToken = hashService.HashDeterministic(request.Token);
        var token = await tokenRepository.GetValidTokenAsync<PasswordResetToken>(hashedToken, cancellationToken);

        if (token is null || token.UserId != user.Id)
        {
            logger.LogWarning("Invalid or expired password reset token for {Email}.", request.Email);

            return Result<MessageResponse>.Failure("Invalid or expired token.", ErrorType.Unauthorized);
        }
        
        token.Revoke("Password Reset Successful");
        
        user.UpdatePassword(request.NewPassword);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Password successfully reset for user {UserId}.", user.Id);
        
        return Result<MessageResponse>.Success(new MessageResponse("You successfully reset your password."));
    }
}