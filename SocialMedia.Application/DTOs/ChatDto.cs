namespace SocialMedia.Application.DTOs;

public class ChatDto
{
    public int ChatId { get; set; }

    public bool? IsGroup { get; set; }

    public string? Title { get; set; }
    
    public string? ReceiverUsername { get; set; }
    
    public string? ReceiverProfilePic { get; set; }
    public string? LastMessageContent { get; set; }
    
    public string? LastMessageUser { get; set; }
    
    public bool? LastMessageFromMe { get; set; }
}