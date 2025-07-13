namespace SocialMedia.Application.DTOs;

public record class UsernameDto
{
    public Guid UserId { get; init; }
    
    public string Username { get; init; } = null!;
};