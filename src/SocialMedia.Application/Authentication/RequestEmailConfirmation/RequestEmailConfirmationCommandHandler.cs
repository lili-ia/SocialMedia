using System.Security.Cryptography;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.Configurations;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Authentication.RequestEmailConfirmation;

public class RequestEmailConfirmationCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ILogger<RequestEmailConfirmationCommandHandler> logger,
    IOptions<ClientSettings> settings,
    IEmailBuilder emailBuilder,
    IPendingEmailRepository emailRepository,
    IHashService hashService,
    ITokenRepository tokenRepository)
    : IRequestHandler<RequestEmailConfirmationCommand, Result<MessageResponse>>
{
    private const string Subject = "Verify your email for SocialMedia"; 
    
    public async Task<Result<MessageResponse>> Handle(RequestEmailConfirmationCommand request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        
        var user = await userRepository.GetByEmailAsync(normalizedEmail, ct, tracking: true);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found.", normalizedEmail);

            return Result<MessageResponse>.Success(new MessageResponse("Success! Check your inbox."));
        }

        if (user.Status != UserStatus.Pending)
        {
            logger.LogWarning("User with email {Email} already confirmed email.", normalizedEmail);
            
            return Result<MessageResponse>.Failure("Email already confirmed", ErrorType.Conflict);
        }
        
        var retryThreshold = DateTime.UtcNow.AddMinutes(-2);
        
        if (user.LastEmailSentAt.HasValue && user.LastEmailSentAt > retryThreshold)
        {
            var secondsToWait = (int)(user.LastEmailSentAt.Value.AddMinutes(2) - DateTime.UtcNow).TotalSeconds;
            
            return Result<MessageResponse>.Failure(
                $"Please wait {secondsToWait} seconds before requesting another email.", 
                ErrorType.TooManyRequests);
        }
        
        user.RecordEmailSent();
        
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        var hashedToken = hashService.HashDeterministic(rawToken);

        var token = EmailConfirmationToken.Create(
            user.Id, 
            hashedToken, 
            DateTime.UtcNow.AddHours(1));
        
        await tokenRepository.AddAsync(token, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        var verificationLink = $"{settings.Value.ClientUrl}/api/auth/confirm-email?token={rawToken}";
        var body = emailBuilder.BuildEmailVerificationBody(user.UsernameNormalized, verificationLink);
        
        var emailSenderResponse = await emailSender.SendEmailAsync(normalizedEmail, Subject, body, ct);

        if (!emailSenderResponse.IsSuccess)
        {
            var pendingEmail = PendingEmail.Create(
                user.EmailNormalized,
                Subject,
                body);

            await emailRepository.AddAsync(pendingEmail, ct);
            
            return Result<MessageResponse>.InternalError("Couldn't send a verification email. Please try later.");
        }
        
        await unitOfWork.SaveChangesAsync(ct);
        
        logger.LogInformation("Confirmation email resent to {Email}.", normalizedEmail);
        
        return Result<MessageResponse>.Success(new MessageResponse("Success! Check your inbox."));
    }
}