using System.Text.Json.Serialization;

namespace SocialMedia.Application.DTOs.Chat;

public class ChatParticipantDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; } = string.Empty;
    
    public string? ThumbnailProfilePicUrl { get; set; }
    
    [JsonIgnore]
    public string? ThumbnailStorageKey { get; set; }
    
    public bool IsAdmin { get; set; }
}