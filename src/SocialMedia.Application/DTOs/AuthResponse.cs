namespace SocialMedia.Application.DTOs;

public class AuthResponse
{
    public string AccessToken { get; init; } = null!;
    
    public string RefreshToken { get; init; } = null!;
};