namespace SocialMedia.Application.DTOs.User;

public class UpdateUserDto
{
    public string Username { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string Email { get; set; } = null!;
    
    public string? ProfilePicUrl { get; set; }
    
    public string? Bio { get; set; }
    
    public DateTime CreatedAt { get; set; }
}