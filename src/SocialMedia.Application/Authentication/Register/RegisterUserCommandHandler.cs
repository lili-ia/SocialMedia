using System.Security.Cryptography;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Common;
using SocialMedia.Application.Common.Configurations;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Application.Authentication.Register;

public class RegisterUserCommandHandler(
    ILogger<RegisterUserCommandHandler> logger,
    IUserRepository userRepository,
    IHashService hashService,
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    IEmailBuilder emailBuilder,
    IOptions<ClientSettings> settings,
    IPendingEmailRepository emailRepository,
    ITokenRepository tokenRepository)
    : IRequestHandler<RegisterUserCommand, Result<MessageResponse>>
{
    private const string Subject = "Verify your email for SocialMedia"; 
    public async Task<Result<MessageResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim().ToLower();
        var normalizedUsername = request.Username.Trim().ToLower();
        
        var existingUser = await userRepository.GetByEmailOrUsernameAsync(normalizedEmail, normalizedUsername, ct);

        if (existingUser is not null)
        {
            return Result<MessageResponse>.Failure(existingUser.EmailNormalized == normalizedEmail
                ? "User with this email already exists." : "User with this username already exists.", ErrorType.Conflict);
        }
        
        var passwordHash = hashService.Hash(request.RawPassword);

        var user = User.Create(
            normalizedUsername, 
            normalizedEmail, 
            passwordHash, 
            DateOnly.FromDateTime(request.BirthDate));
        
        await userRepository.AddAsync(user, ct);
        
        logger.LogInformation("User with email {Email} registered successfully. UserId: {UserId}", normalizedEmail, user.Id);
        
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        var hashedToken = hashService.HashDeterministic(rawToken);

        var token = EmailConfirmationToken.Create(
            user.Id, 
            hashedToken, 
            DateTime.UtcNow.AddHours(1));
        
        await tokenRepository.AddAsync(token, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        var verificationLink = $"{settings.Value.ClientUrl}/api/auth/confirm-email?token={rawToken}" +
                               $"&email={Uri.EscapeDataString(user.EmailNormalized)}";
        var body = emailBuilder.BuildEmailVerificationBody(normalizedUsername, verificationLink);
        
        var emailSenderResponse = await emailSender.SendEmailAsync(normalizedEmail, Subject, body, ct);

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

        var response = new MessageResponse("User was successfully registered. Now check your inbox to confirm email.");
        
        return Result<MessageResponse>.Success(response);
    }
}