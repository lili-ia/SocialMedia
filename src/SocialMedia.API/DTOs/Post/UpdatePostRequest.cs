namespace SocialMedia.DTOs.Post;

public class UpdatePostRequest
{
    public string? Text { get; set; } = null!;

    public List<IFormFile>? NewFiles { get; set; }
    
    public List<string>? KeptStorageKeys { get; set; }
}