using System.Security.Cryptography;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Shared.ResultPattern;

namespace SocialMedia.Application.Authentication.RequestEmailConfirmation;

public class RequestEmailConfirmationCommandHandler : IRequestHandler<RequestEmailConfirmationCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<RequestEmailConfirmationCommandHandler> _logger;
    private readonly IValidator<RequestEmailConfirmationCommand> _validator;

    public RequestEmailConfirmationCommandHandler(
        IUserRepository userRepository,
        ITokenRepository tokenRepository,
        IUnitOfWork unitOfWork,
        IEmailSender emailSender,
        IPasswordService passwordService,
        ILogger<RequestEmailConfirmationCommandHandler> logger, 
        IValidator<RequestEmailConfirmationCommand> validator)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _passwordService = passwordService;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result> Handle(RequestEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User with email {Email} requests email confirmation token.", request.Email);

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

        if (user.Status != UserStatus.Pending)
        {
            _logger.LogWarning("User with email {Email} already confirmed email.", normalizedEmail);
            
            return Result<bool>.Failure("Email already confirmed", ErrorType.Conflict);
        }

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashedToken = _passwordService.HashPassword(rawToken);

        var token = new EmailConfirmationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = hashedToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            IsRevoked = false
        };

        try
        {
            await _tokenRepository.AddAsync(token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var body = $"<p>Please confirm your email by clicking the link below:</p>" +
                       $"<p><a href='https://example.com/confirm-email?token={Uri.EscapeDataString(rawToken)}&email={Uri.EscapeDataString(normalizedEmail)}'>Confirm Email</a></p>";

            await _emailSender.SendEmailAsync(normalizedEmail, "Confirm your email", body);

            _logger.LogInformation("Email confirmation token sent to {Email}.", normalizedEmail);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email confirmation token to {Email}", normalizedEmail);
            
            return Result<bool>.Failure("An internal error occurred", ErrorType.ServerError);
        }
    }
}