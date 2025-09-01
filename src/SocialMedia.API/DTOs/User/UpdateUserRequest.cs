namespace SocialMedia.DTOs.User;

public class UpdateUserRequest
{
    public DateTime? BirthDate { get; set; }
    
    public IFormFile? ProfilePic { get; set; }
    
    public string? Bio { get; set; }
}