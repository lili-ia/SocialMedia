using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    public Guid RecipientId { get; set; }

    public NotificationType Type { get; set; } = NotificationType.System;

    public bool IsRead { get; set; } = false;
    
    public DateTime Timestamp { get; set; } = DateTime.Now;
    
    public Dictionary<string, string> Data { get; set; } =  new();
    
    public virtual User Recipient { get; set; }
}