using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; set; }
    
    public bool IsRead { get; set; }
    
    public DateTime? ReadAt { get; set; }
    
    public Dictionary<string, string> Data { get; set; } = [];
    
    public Guid RecipientId { get; set; }

    public User Recipient { get; set; } = null!;
}