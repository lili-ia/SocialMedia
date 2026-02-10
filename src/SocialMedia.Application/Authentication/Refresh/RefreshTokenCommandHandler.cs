using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Auth;

namespace SocialMedia.Application.Authentication.Refresh;

public class RefreshTokenCommandHandler(
    IValidator<RefreshTokenCommand> validator,
    ILogger<RefreshTokenCommandHandler> logger,
    IUserRepository userRepository,
    ITokenService tokenService,
    ITokenRepository tokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to refresh token {RefreshToken}.", request.RefreshToken);

        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult<AuthResponse>();
        }
        
        var token = await tokenRepository.GetValidTokenAsync<RefreshToken>(request.RefreshToken, cancellationToken);

        if (token is null || !token.IsActive)
        {
            logger.LogWarning("Expired or invalid refresh token attempt. IP: {IP}, Device: {Device}", 
                request.IpAddress, request.DeviceInfo);
            
            if (token is { IsUsed: true })
            {
                logger.LogCritical("Used refresh token replay detected for User {UserId}. Revoking all tokens.", token.UserId);
                await tokenRepository.RevokeAllUserTokensAsync<RefreshToken>(token.UserId, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            return Result<AuthResponse>.Failure("Invalid refresh token", ErrorType.Unauthorized);
        }

        var user = await userRepository.GetByIdAsync(token.UserId, cancellationToken);
        
        if (user is null)
        {
            logger.LogWarning("User with id {UserId} not found.", token.UserId);
            
            return Result<AuthResponse>.Failure("Invalid login attempt.", ErrorType.Unauthorized);
        }
        
        var newRefreshTokenString = tokenService.GenerateRefreshToken();
    
        var newRefreshToken = new RefreshToken
        {
            Token = newRefreshTokenString,
            IsRevoked = false,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        token.IsRevoked = true;
        token.IsUsed = true;
        token.RevokedAt = DateTime.UtcNow;
        token.ReasonForRevocation = "Refreshed";
        token.ReplacedByToken = newRefreshTokenString;

        await tokenRepository.AddAsync(newRefreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await unitOfWork.CommitTransactionAsync(cancellationToken);

        var newAccessToken = tokenService
            .GenerateAccessToken(token.UserId.ToString(), user.EmailNormalized, user.UserRole.ToString());
        
        var authResponse = new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };
        
        return Result<AuthResponse>.Success(authResponse);
    }
}