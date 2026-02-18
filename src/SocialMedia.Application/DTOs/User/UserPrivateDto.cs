namespace SocialMedia.Application.DTOs.User;

public class UserPrivateDto : UserPublicDto
{
    public DateOnly BirthDate { get; set; }
    
    public string Email { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}