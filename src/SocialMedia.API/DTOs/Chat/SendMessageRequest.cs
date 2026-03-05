namespace SocialMedia.DTOs.Chat;

public class SendMessageRequest
{
    public string? Content { get; set; }

    public Guid? ParentMessageId { get; set; }

    public List<IFormFile>? Files { get; set; }
}