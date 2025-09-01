using Domain.Enums;

namespace Domain.Entities;

public sealed class Message : BaseEntity
{
    public string Content { get; set; } = null!;
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public MessageType MessageType { get; set; }
    
    public bool IsEdited { get; set; }
    
    public bool IsRead { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public Guid SenderId { get; set; }
    
    public User Sender { get; set; } = null!;

    public Guid ChatId { get; set; }

    public Chat Chat { get; set; } = null!;

    public Guid? ReplyToMessageId { get; set; }

    public Message? ReplyToMessage { get; set; }
}