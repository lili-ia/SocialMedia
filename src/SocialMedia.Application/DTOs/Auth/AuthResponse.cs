namespace SocialMedia.Application.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; init; } = null!;
    
    public string RefreshToken { get; init; } = null!;
};