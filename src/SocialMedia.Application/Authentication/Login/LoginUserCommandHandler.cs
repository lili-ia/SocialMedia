using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Auth;

namespace SocialMedia.Application.Authentication.Login;

public class LoginUserCommandHandler(
    IValidator<LoginUserCommand> validator,
    ILogger<LoginUserCommandHandler> logger,
    IUserRepository userRepository,
    IHashService hashService,
    ITokenService tokenService,
    ITokenRepository tokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to log in user with email {Email}.", request.Email);

        var validationResult = validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult<AuthResponse>();
        }

        var normalizedEmail = request.Email.Trim().ToLower();
        var existingUser = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existingUser is null)
        {
            logger.LogWarning("Failed login attempt: email {Email}, IP {IP}, Device {Device}", 
                normalizedEmail, request.IpAddress, request.DeviceInfo);
            
            return Result<AuthResponse>.Failure("Invalid email or password.", ErrorType.Unauthorized);
        }
        
        var isPasswordValid = hashService.Verify(existingUser.PasswordHash, request.Password);

        if (!isPasswordValid)
        {
            logger.LogWarning("Failed login attempt: email {Email}, IP {IP}, Device {Device}", 
                normalizedEmail, request.IpAddress, request.DeviceInfo);
            
            return Result<AuthResponse>.Failure("Invalid email or password.", ErrorType.Unauthorized);
        }

        var accessToken = tokenService
            .GenerateAccessToken(existingUser.Id.ToString(), normalizedEmail, existingUser.UserRole.ToString());
        
        var refreshTokenString = tokenService.GenerateRefreshToken();
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenString,
            UserId = existingUser.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IpAddress = request.IpAddress,
            DeviceInfo = request.DeviceInfo
        };
        
        await tokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("User with email {Email} successfully logged in.", existingUser.EmailNormalized);
        
        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
        
        return Result<AuthResponse>.Success(authResponse);
    }
}