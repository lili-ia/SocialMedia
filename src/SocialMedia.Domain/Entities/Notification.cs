using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; set; }
    
    public bool IsRead { set; get; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    public Dictionary<string, string> Data { get; set; } = [];
    
    public Guid RecipientId { get; set; }

    public virtual User Recipient { get; set; } = null!;
}