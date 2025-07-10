namespace SocialMedia.Application.DTOs;

public class PrivateUserProfileDto
{
    public string? Username { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Email { get; set; }
    
    public string? ProfilePicUrl { get; set; }

    public string? Bio { get; set; }

    public int? Status { get; set; }
}