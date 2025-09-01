namespace SocialMedia.Application.DTOs.User;

public class UserPreviewDto
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    
    public string? ProfilePicUrl { get; set; }
}