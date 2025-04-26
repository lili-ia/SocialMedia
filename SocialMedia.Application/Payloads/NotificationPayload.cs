namespace SocialMedia.Application.Payloads;

public class NotificationPayload
{
    public string Type { get; set; } = default!;
    
    public int RecepientUserId { get; set; }
    
    public Dictionary<string, object> Data { get; set; } = new();
}