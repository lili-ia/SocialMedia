namespace SocialMedia.Application.DTOs.User;

public class PrivateUserProfileDto : UserProfileDto
{
    public DateTime? BirthDate { get; set; }

    public string Email { get; set; } = null!;
}