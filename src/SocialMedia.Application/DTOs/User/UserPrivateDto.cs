namespace SocialMedia.Application.DTOs.User;

public class UserPrivateDto : UserPublicDto
{
    public DateTime BirthDate { get; set; }
    
    public string Email { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
}