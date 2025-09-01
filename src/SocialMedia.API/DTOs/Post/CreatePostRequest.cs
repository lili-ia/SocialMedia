namespace SocialMedia.DTOs.Post;

public class CreatePostRequest
{
    public string? Text { get; set; }
    
    public List<IFormFile>? Files { get; set; }
}