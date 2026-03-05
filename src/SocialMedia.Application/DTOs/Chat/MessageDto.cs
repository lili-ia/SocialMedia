using System.Text.Json.Serialization;
using Domain.Enums;

namespace SocialMedia.Application.DTOs.Chat;

public class MessageDto
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }
    
    public Guid SenderId { get; set; }
    
    public string SenderUsername { get; set; } = string.Empty;
    
    public string? SenderThumbnailProfilePicUrl { get; set; }
    
    [JsonIgnore]
    public string? SenderThumbnailProfilePicStorageKey { get; set; }
    
    public string? Text { get; set; }
    
    public MessageStatus Status { get; set; }
    
    public Guid? ParentMessageId { get; set; }
    
    public List<MessageAttachmentDto> Attachments { get; set; } = [];
    
    public DateTime CreatedAt { get; set; }
}