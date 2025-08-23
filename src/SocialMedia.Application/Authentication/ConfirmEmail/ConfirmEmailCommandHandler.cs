using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;

    public ConfirmEmailCommandHandler(
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ILogger<ConfirmEmailCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to confirm email {Email}.", request.Email);

        var normalizedEmail = request.Email.Trim().ToLower();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", normalizedEmail);
            
            return Result.Failure("Invalid email or token", ErrorType.BadRequest);
        }

        var hashedToken = _passwordService.HashPassword(request.Token);

        var token = await _tokenRepository.GetValidTokenAsync<EmailConfirmationToken>(hashedToken, cancellationToken);

        if (token == null)
        {
            _logger.LogWarning("Invalid or expired token for user {Email}.", normalizedEmail);
            
            return Result.Failure("Invalid or expired token.", ErrorType.Forbidden);
        }

        if (user.Status != UserStatus.Pending)
        {
            return Result<bool>.Failure("Email already confirmed.", ErrorType.Conflict);
        }

        user.Status = UserStatus.Active;
        token.IsRevoked = true;

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Email confirmed for user {Email}.", normalizedEmail);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for user {Email}.", normalizedEmail);
            
            return Result<bool>.Failure("An internal error occurred.", ErrorType.ServerError);
        }
    }
}