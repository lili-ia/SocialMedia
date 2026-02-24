using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Authentication.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<ConfirmEmailCommandHandler> logger,
    ITokenRepository tokenRepository,
    IHashService hashService)
    : IRequestHandler<ConfirmEmailCommand, Result<MessageResponse>>
{
    public async Task<Result<MessageResponse>> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var hashedToken = hashService.HashDeterministic(request.Token);
        var token = await tokenRepository.GetValidTokenAsync<EmailConfirmationToken>(hashedToken, ct);

        if (token is null)
        {
            return Result<MessageResponse>.Failure("Token is expired or already used.", ErrorType.Unauthorized);
        }
        
        var user = await userRepository.GetByIdAsync(token.UserId, ct, tracking: true);
        
        if (user is null)
        {
            return Result<MessageResponse>.Failure("User not found.", ErrorType.NotFound);
        }

        var normalizedEmail = request.Email.Trim().ToLower();
        
        if (user.EmailNormalized != normalizedEmail)
        {
            return Result<MessageResponse>.Failure(
                "This link is no longer valid for your current email.", 
                ErrorType.Unauthorized);
        }
        
        if (user.Status != UserStatus.Pending)
        {
            return Result<MessageResponse>.Success(new MessageResponse("You already confirmed your email."));
        }
        
        user.ChangeStatus(UserStatus.Active, "Email confirmed.");
        token.Revoke("Email Confirmation Successful");
        
        await unitOfWork.SaveChangesAsync(ct); 
    
        logger.LogInformation("User {UserId} successfully verified email.", user.Id);
        
        return Result<MessageResponse>.Success(new MessageResponse("You successfully confirmed your email."));
    }
}