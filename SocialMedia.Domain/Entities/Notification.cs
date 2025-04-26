namespace Domain.Entities;

public class Notification
{
    public int NotificationId { get; set; }
    
    public int RecipientId { get; set; }
    
    public string Type { get; set; }
    
    public bool IsRead { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public Dictionary<string, string> Data { get; set; }
    
    public virtual User Recipient { get; set; }
}