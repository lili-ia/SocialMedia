using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IValidator<RefreshTokenCommand> _validator;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RefreshTokenCommandHandler(
        IValidator<RefreshTokenCommand> validator, 
        ILogger<RefreshTokenCommandHandler> logger, 
        IUserRepository userRepository, 
        IPasswordService passwordService, 
        IJwtService jwtService, 
        ITokenRepository tokenRepository, 
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _logger = logger;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to refresh token {RefreshToken}.", request.RefreshToken);

        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult<AuthResponse>();
        }
        
        var token = await _tokenRepository.GetValidTokenAsync<RefreshToken>(request.RefreshToken, cancellationToken);

        if (token is null)
        {
            _logger.LogWarning("Expired or invalid refresh token attempt. IP: {IP}, Device: {Device}", 
                request.IpAddress, request.DeviceInfo);
            
            return Result<AuthResponse>.Failure("Invalid refresh token", ErrorType.Unauthorized);
        }

        var user = await _userRepository.GetByIdAsync(token.UserId, cancellationToken);
        
        if (user is null)
        {
            _logger.LogWarning("User with id {UserId} not found.", token.UserId);
            
            return Result<AuthResponse>.Failure("Invalid login attempt.", ErrorType.Unauthorized);
        }

        var newAccessToken = _jwtService
            .GenerateToken(token.UserId.ToString(), user.Email, user.UserRole.ToString());
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();
        
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            IpAddress = request.IpAddress,
            DeviceInfo = request.DeviceInfo
        };

        token.IsRevoked = true;

        try
        {
            await _tokenRepository.UpdateAsync(token, cancellationToken);
            await _tokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Refresh token for user with id {UserId} was successfully created.", user.Id);
            
            var authResponse = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

            return Result<AuthResponse>.Success(authResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating token for user with id {UserId}.", user.Id);
            
            return Result<AuthResponse>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}