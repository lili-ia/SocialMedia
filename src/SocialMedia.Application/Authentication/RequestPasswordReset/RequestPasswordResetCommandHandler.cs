using System.Security.Cryptography;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.RequestPasswordReset;

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Result>
{
    private readonly IValidator<RequestPasswordResetCommand> _validator;
    private readonly ILogger<RequestPasswordResetCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenRepository _tokenRepository;
    private readonly IEmailSender _emailSender;
    
    public RequestPasswordResetCommandHandler(
        IValidator<RequestPasswordResetCommand> validator, 
        ILogger<RequestPasswordResetCommandHandler> logger, 
        IUserRepository userRepository, 
        IPasswordService passwordService, 
        IUnitOfWork unitOfWork, 
        ITokenRepository tokenRepository, 
        IEmailSender emailSender)
    {
        _validator = validator;
        _logger = logger;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
        _tokenRepository = tokenRepository;
        _emailSender = emailSender;
    }
    
    public async Task<Result> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User with email {Email} requests password reset token.", request.Email);

        var validationResult = _validator.Validate(request);
        
        if (!validationResult.IsValid)
        {
            return validationResult.ToFailureResult();
        }

        var normalizedEmail = request.Email.Trim().ToLower();
        
        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User with email {Email} not found.", normalizedEmail);
            
            return Result.Success();
        }
        
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashedToken = _passwordService.HashPassword(rawToken);

        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = hashedToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsRevoked = false
        };

        try
        {
            await _tokenRepository.AddAsync(token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var body = $"<p>Reset password using this link:</p>" +
                       $"<p><a href='https://example.com/reset-password?token={Uri.EscapeDataString(rawToken)}&email={Uri.EscapeDataString(user.Email)}'>Reset Password</a></p>";

            await _emailSender.SendEmailAsync(user.Email, "Reset your password", body);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to user {Email}.", user.Email);
            
            return Result.Failure("An internal error occurred.", ErrorType.ServerError);
        }
    }
}