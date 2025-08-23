namespace SocialMedia.DTOs.Auth;

public class ConfirmEmailRequest
{
    public string Email { get; init; } = null!;

    public string Token { get; init; } = null!;
}