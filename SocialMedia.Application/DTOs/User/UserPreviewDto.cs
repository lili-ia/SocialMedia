namespace SocialMedia.Application.DTOs.User;

public record class UserPreviewDto
{
    public Guid UserId { get; init; }
    
    public string Username { get; init; } = null!;
}