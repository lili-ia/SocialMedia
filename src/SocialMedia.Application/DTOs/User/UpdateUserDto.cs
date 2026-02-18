namespace SocialMedia.Application.DTOs.User;

public class UpdateUserDto
{
    public string Username { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string Email { get; set; } = null!;
    
    public string? ProfilePicUrl { get; set; }
    
    public string? Bio { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}