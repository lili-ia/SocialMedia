using Domain.Entities;
using SocialMedia.Application.DTOs;
using SocialMedia.Application.Requests;

namespace SocialMedia.Application.Contracts;

public interface IAuthService
{
    Task<Result<User>> RegisterAsync(RegisterDto dto, CancellationToken ct);
    
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken ct);

    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken ct);

    Task<Result<bool>> RequestPasswordResetAsync(string email);

    Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request);

    Task<Result<bool>> SendEmailConfirmationAsync(string email);

    Task<Result<bool>> ConfirmEmailAsync(string email, string token);
}