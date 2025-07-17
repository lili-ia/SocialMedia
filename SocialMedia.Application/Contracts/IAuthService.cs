using SocialMedia.Application.DTOs.Auth;

namespace SocialMedia.Application.Contracts;

public interface IAuthService
{
    Task<Result<RegisterResponse>> RegisterAsync(RegisterDto dto, CancellationToken ct);
    
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, string ipAddress, string deviceInfo, CancellationToken ct);

    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string ipAddress, string deviceInfo, CancellationToken ct);

    Task<Result<bool>> RequestPasswordResetAsync(string email);

    Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request);

    Task<Result<bool>> SendEmailConfirmationAsync(string email);

    Task<Result<bool>> ConfirmEmailAsync(string email, string token);
}