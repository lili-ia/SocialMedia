using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Authentication.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly IValidator<ResetPasswordCommand> _validator;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<ResetPasswordCommandHandler> logger,
        IValidator<ResetPasswordCommand> validator)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User with email {Email} requests password reset token.", request.Email);

        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult();
        }
        
        var normalizedEmail = request.Email.Trim().ToLower();

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", normalizedEmail);

            return Result.Success();
        }

        var hashedToken = _passwordService.HashPassword(request.Token);

        var token = await _tokenRepository.GetValidTokenAsync<PasswordResetToken>(hashedToken, cancellationToken);

        if (token == null)
        {
            _logger.LogWarning("Invalid or expired password reset token for {Email}.", request.Email);

            return Result.Failure("Invalid or expired token.", ErrorType.Unauthorized);
        }

        token.IsRevoked = true;
        user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
        
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Password successfully reset for {Email}.", request.Email);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for {Email}.", request.Email);

            return Result.Failure("An internal error occurred.", ErrorType.ServerError);
        }
    }
}