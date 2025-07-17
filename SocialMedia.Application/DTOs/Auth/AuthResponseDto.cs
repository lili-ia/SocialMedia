namespace SocialMedia.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; init; } = null!;
    
    public string RefreshToken { get; init; } = null!;
};