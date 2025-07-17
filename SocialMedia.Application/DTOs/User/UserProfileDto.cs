namespace SocialMedia.Application.DTOs.User;

public class UserProfileDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = null!;
    
    public string? ProfilePicUrl { get; set; }

    public string? Bio { get; set; }
}