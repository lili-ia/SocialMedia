namespace SocialMedia.Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; init; } = null!;
    
    public string RefreshToken { get; init; } = null!;
};