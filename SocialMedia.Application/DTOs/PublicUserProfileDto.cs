namespace SocialMedia.Application.DTOs;

public class PublicUserProfileDto
{
    public string? Username { get; set; }

    public DateTime? BirthDate { get; set; }
    
    public string? ProfilePicUrl { get; set; }

    public string? Bio { get; set; }

    public int? Status { get; set; }
}