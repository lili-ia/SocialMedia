namespace SocialMedia.DTOs.User;

public class UpdateUserRequest
{
    public DateOnly? BirthDate { get; set; }
    
    public IFormFile? NewProfilePic { get; set; }

    public string? KeptProfilePicStorageKey { get; set; }
    
    public string? Bio { get; set; }
}