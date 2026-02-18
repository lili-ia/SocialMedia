using System.Text.Json.Serialization;

namespace SocialMedia.Application.DTOs.User;

public class UserPreviewDto
{
    public Guid Id { get; set; }
    
    public string Username { get; set; }
    
    public string? ThumbnailProfilePicUrl { get; set; }
    
    [JsonIgnore]
    public string? ThumbnailProfilePicStorageKey { get; set; }
}