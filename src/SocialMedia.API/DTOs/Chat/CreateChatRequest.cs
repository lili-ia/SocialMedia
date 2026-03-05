namespace SocialMedia.DTOs.Chat;

public class CreateChatRequest
{
    public Guid RequesterId { get; set; }
    
    public bool IsGroup { get; set; }
    
    public string? GroupName { get; set; }

    public List<Guid> ParticipantIds { get; set; } = [];
}