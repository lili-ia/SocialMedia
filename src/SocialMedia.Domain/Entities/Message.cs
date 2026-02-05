using Domain.Enums;

namespace Domain.Entities;

public sealed class Message : BaseEntity
{
    public string? Content { get; set; }
    
    public MessageType MessageType { get; set; }
    
    public bool IsEdited { get; set; }
    
    public bool IsRead { get; set; }
    
    public Guid SenderId { get; set; }
    
    public User Sender { get; set; } = null!;

    public Guid ChatId { get; set; }

    public Chat Chat { get; set; } = null!;
    
    public Guid? ParentMessageId { get; set; }

    public Message? ParentMessage { get; set; }
    
    public ICollection<MessageAttachment> Attachments { get; set; } = [];
}