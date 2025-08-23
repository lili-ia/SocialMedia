namespace SocialMedia.DTOs.Auth;

public class ResetPasswordRequest
{
    public string Email { get; init; } = null!;

    public string Token { get; init; } = null!;

    public string NewPassword { get; init; } = null!;
}