namespace SocialMedia.DTOs.Auth;

public record class EmailConfirmationRequest
{
    public string Email { get; init; } = null!;
}