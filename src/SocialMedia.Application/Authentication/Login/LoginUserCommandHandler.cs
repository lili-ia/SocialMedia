using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.Auth;

namespace SocialMedia.Application.Authentication.Login;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    private readonly IValidator<LoginUserCommand> _validator;
    private readonly ILogger<LoginUserCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public LoginUserCommandHandler(
        IValidator<LoginUserCommand> validator, 
        ILogger<LoginUserCommandHandler> logger, 
        IUserRepository userRepository, 
        IPasswordService passwordService, 
        IJwtService jwtService, 
        ITokenRepository tokenRepository, 
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _logger = logger;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to log in user with email {Email}.", request.Email);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult<AuthResponse>();
        }

        var normalizedEmail = request.Email.Trim().ToLower();
        
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existingUser is null)
        {
            _logger.LogWarning("Failed login attempt: email {Email}, IP {IP}, Device {Device}", 
                normalizedEmail, request.IpAddress, request.DeviceInfo);
            
            return Result<AuthResponse>.Failure("Invalid email or password.", ErrorType.Unauthorized);
        }
        
        var isPasswordValid = _passwordService.VerifyPassword(existingUser.PasswordHash, request.Password);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Failed login attempt: email {Email}, IP {IP}, Device {Device}", 
                normalizedEmail, request.IpAddress, request.DeviceInfo);
            
            return Result<AuthResponse>.Failure("Invalid email or password.", ErrorType.Unauthorized);
        }

        var accessToken = _jwtService
            .GenerateToken(existingUser.Id.ToString(), normalizedEmail, existingUser.UserRole.ToString());
        
        var refreshTokenString = _jwtService.GenerateRefreshToken();
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenString,
            UserId = existingUser.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            IpAddress = request.IpAddress,
            DeviceInfo = request.DeviceInfo
        };

        try
        {
            await _tokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("User with email {Email} successfully logged in.", existingUser.Email);
            
            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
            
            return Result<AuthResponse>.Success(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user with email {Email}.", request.Email);
            
            return Result<AuthResponse>.Failure("An internal error occured.", ErrorType.ServerError);
        }
    }
}