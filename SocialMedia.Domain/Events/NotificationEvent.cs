namespace Domain.Events;

public class NotificationEvent
{
    public string Type { get; set; }  
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}