using System.Security.Cryptography;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.Configurations;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Authentication.RequestPasswordReset;

public class RequestPasswordResetCommandHandler(
    ILogger<RequestPasswordResetCommandHandler> logger,
    IUserRepository userRepository,
    IHashService hashService,
    IUnitOfWork unitOfWork,
    ITokenRepository tokenRepository,
    IEmailSender emailSender,
    IEmailBuilder emailBuilder,
    IPendingEmailRepository emailRepository,
    IOptions<ClientSettings> clientSettings) 
    : IRequestHandler<RequestPasswordResetCommand, Result<MessageResponse>> 
{
    private readonly string Subject = "Reset your password for SocialMedia";
    
    public async Task<Result<MessageResponse>> Handle(RequestPasswordResetCommand request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        
        var user = await userRepository.GetByEmailAsync(normalizedEmail, ct, tracking: true);

        if (user is null)
        {
            logger.LogWarning("User with email {Email} not found.", normalizedEmail);
            
            return Result<MessageResponse>.Success(
                new MessageResponse("If your account exists, you'll get an email with reset instructions."));
        }
        
        if (!user.CanSendEmail())
        {
            var secondsToWait = (int)(user.LastEmailSentAt!.Value.AddMinutes(2) - DateTime.UtcNow).TotalSeconds;

            if (secondsToWait > 0)
            {
                return Result<MessageResponse>.Failure(
                    $"Please wait {secondsToWait} seconds before requesting another email.", 
                    ErrorType.TooManyRequests);
            }
        }
        
        user.RecordEmailSent();
        
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        var hashedToken = hashService.HashDeterministic(rawToken);

        var token = PasswordResetToken.Create(
            user.Id, 
            hashedToken, 
            DateTime.UtcNow.AddHours(1));
        
        await tokenRepository.AddAsync(token, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var resetLink = $"{clientSettings.Value.ClientUrl}/api/auth/reset-password?" + 
            $"token={Uri.EscapeDataString(rawToken)}&email={Uri.EscapeDataString(user.EmailNormalized)}";

        var body = emailBuilder.BuildPasswordResetBody(user.UsernameNormalized, resetLink);
        
        var emailSenderResponse = await emailSender.SendEmailAsync(user.EmailNormalized, Subject, body, ct);
        
        if (!emailSenderResponse.IsSuccess)
        {
            var pendingEmail = PendingEmail.Create(
                user.EmailNormalized,
                Subject,
                body);

            await emailRepository.AddAsync(pendingEmail, ct);
            await unitOfWork.SaveChangesAsync(ct);
            
            return Result<MessageResponse>.InternalError("Couldn't send a verification email. Please try later.");
        }
        
        return Result<MessageResponse>.Success(
            new MessageResponse("If your account exists, you'll get an email with reset instructions."));
    }
}