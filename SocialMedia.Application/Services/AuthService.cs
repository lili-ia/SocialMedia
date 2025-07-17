using System.Security.Cryptography;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.DTOs.Auth;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class AuthService : IAuthService
{
    private readonly SocialMediaContext _db;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtService _jwtService;
    private readonly IEmailSender _emailSender;
    
    public AuthService(
        SocialMediaContext db, 
        IPasswordService passwordService, 
        ILogger<AuthService> logger, 
        IJwtService jwtService, 
        IEmailSender sender)
    {
        _db = db;
        _passwordService = passwordService;
        _logger = logger;
        _jwtService = jwtService;
        _emailSender = sender;
    }
    
    public async Task<Result<RegisterResponse>> RegisterAsync(RegisterDto registerDto, CancellationToken ct)
    {
        _logger.LogInformation("Attempting to register a user with email {Email}.", registerDto.Email);
        
        var userExists = await _db.Users.AnyAsync(u => u.Email == registerDto.Email, ct);
        
        if (userExists)
        {
            _logger.LogWarning("User with email {Email} already exists.", registerDto.Email);
            
            return Result<RegisterResponse>.FailureResult($"User with email {registerDto.Email} already exists", ErrorType.Validation);
        }

        var newUser = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email, 
            PasswordHash = _passwordService.HashPassword(registerDto.RawPassword)
        };

        try
        {
            await _db.Users.AddAsync(newUser, ct);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with email {Email} successfully registered.", registerDto.Email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error registering user with email {Email}.", registerDto.Email);
            
            return Result<RegisterResponse>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }

        var newUserDto = new RegisterResponse
        {
            Id = newUser.Id,
            Username = newUser.Username,
            Email = registerDto.Email, 
        };
        
        return Result<RegisterResponse>.SuccessResult(newUserDto);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto, string ipAddress, string deviceInfo, CancellationToken ct)
    {
        _logger.LogInformation("User with email {Email} attempts to log in.", loginDto.Email);
        
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email, ct);

        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", loginDto.Email);
            
            return Result<AuthResponseDto>.FailureResult("Invalid login attempt.", ErrorType.Validation);
        }

        var isPasswordValid = _passwordService.VerifyPassword(user.PasswordHash, loginDto.RawPassword);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Password not valid.");
            
            return Result<AuthResponseDto>.FailureResult("Invalid login attempt.", ErrorType.Validation);
        }

        var accessToken = _jwtService.GenerateToken(user.Id.ToString(), user.Email);
        
        var refreshTokenString = _jwtService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenString,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo
        };

        try
        {
            await _db.RefreshTokens.AddAsync(refreshToken, ct);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("User with email {Email} successfully logged in.", user.Email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error logging in user with email {Email}.", user.Email);
            
            return Result<AuthResponseDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }
        
        var authResponse = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
        
        return Result<AuthResponseDto>.SuccessResult(authResponse);
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string ipAddress, string deviceInfo, CancellationToken ct)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken, ct);

        if (token == null || token.Expires < DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token doesn't exist or expired.");
            
            return Result<AuthResponseDto>.FailureResult("Invalid refresh token", ErrorType.Unauthorized);
        }

        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == token.UserId, ct);
        
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} not found.", token.UserId);
            
            return Result<AuthResponseDto>.FailureResult("User not found.", ErrorType.NotFound);
        }

        var newAccessToken = _jwtService.GenerateToken(token.UserId.ToString(), user.Email);
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();
        
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenString,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo
        };

        token.IsRevoked = true;

        try
        {
            _db.RefreshTokens.Update(token);
            await _db.RefreshTokens.AddAsync(newRefreshToken, ct);
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Refresh token for user with id {UserId} was successfully created.", user.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating token for user with id {USerId}.", user.Id);
            
            return Result<AuthResponseDto>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }

        var authResponse = new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token
        };

        return Result<AuthResponseDto>.SuccessResult(authResponse);
    }

    public Task<Result<bool>> RequestPasswordResetAsync(string email)
    {
        return SendTokenEmailAsync<PasswordResetToken>(
            email,
            _db.PasswordResetTokens,
            TimeSpan.FromHours(1),
            "Password Reset Request",
            (rawToken, userEmail) => 
                $"<p>To reset your password, click the link below:</p>" +
                $"<p><a href='https://example.com/reset-password?token={Uri.EscapeDataString(rawToken)}&email={Uri.EscapeDataString(userEmail)}'>Reset Password</a></p>"
        );
    }
    
    public Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return ValidateAndConsumeTokenAsync<PasswordResetToken>(request.Email, request.Token, async user =>
        {
            user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            
            return Result<bool>.SuccessResult(true);
        });
    }
    
    public Task<Result<bool>> SendEmailConfirmationAsync(string email)
    {
        return SendTokenEmailAsync<EmailConfirmationToken>(
            email,
            _db.EmailConfirmationTokens,
            TimeSpan.FromDays(1),
            "Confirm your email",
            (rawToken, userEmail) =>
                $"<p>Please confirm your email by clicking the link below:</p>" +
                $"<p><a href='https://example.com/confirm-email?token={Uri.EscapeDataString(rawToken)}&email={Uri.EscapeDataString(userEmail)}'>Confirm Email</a></p>",
            user => user.Status == UserStatus.Pending
        );
    }

    public Task<Result<bool>> ConfirmEmailAsync(string email, string token)
    {
        return ValidateAndConsumeTokenAsync<EmailConfirmationToken>(email, token, async user =>
        {
            if (user.Status != UserStatus.Pending)
            {
                return Result<bool>.FailureResult("Email already confirmed", ErrorType.BadRequest);
            }
            
            user.Status = UserStatus.Active;
            
            return Result<bool>.SuccessResult(true);
        });
    }
    
    private async Task<Result<bool>> SendTokenEmailAsync<TToken>(
        string email,
        DbSet<TToken> tokenDbSet,
        TimeSpan tokenLifetime,
        string subject,
        Func<string, string, string> createBody, 
        Func<User, bool>? additionalUserCheck = null
    ) where TToken : UserTokenBase, new()
    {
        _logger.LogInformation("User with email {Email} attempts to request token for email confirmation.", email);
        
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", email);
            
            return Result<bool>.SuccessResult(true);
        }

        if (additionalUserCheck != null && !additionalUserCheck(user))
        {
            return Result<bool>.FailureResult("Condition not met", ErrorType.BadRequest);
        }
        
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashedToken = _passwordService.HashPassword(rawToken);

        var token = new TToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = hashedToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(tokenLifetime),
            IsUsed = false
        };

        try
        {
            await tokenDbSet.AddAsync(token);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Email confirmation token for user with email {Email} was successfully added to database.", email);
            
            var body = createBody(rawToken, email);
            await _emailSender.SendEmailAsync(email, subject, body);
            
            _logger.LogInformation("Email confirmation token was successfully sent to user with email {Email}.", email);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending email confirmation token to user with email {Email}.", email);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }

        return Result<bool>.SuccessResult(true);
    }
    
    private async Task<Result<bool>> ValidateAndConsumeTokenAsync<TToken>(
        string email,
        string tokenRaw,
        Func<User, Task<Result<bool>>> onSuccess
    ) where TToken : UserTokenBase
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            _logger.LogWarning("User with email {Email} not found.", email);
            
            return Result<bool>.FailureResult("Invalid email or token", ErrorType.BadRequest);
        }
        
        var hashedToken = _passwordService.HashPassword(tokenRaw);

        var token = await _db.Set<TToken>()
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.Token == hashedToken && !t.IsUsed);

        if (token == null || token.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid or expired token for user with email {Email}.", email);
            
            return Result<bool>.FailureResult("Invalid or expired token", ErrorType.Forbidden);
        }
        
        token.IsUsed = true;

        try
        {
            var result = await onSuccess(user);

            if (!result.Success)
            {
                return result;
            }
            
            await _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error validating token for user with email {Email}.", email);
            
            return Result<bool>.FailureResult("An internal error occured.", ErrorType.ServerError);
        }

        return Result<bool>.SuccessResult(true);
    }
}