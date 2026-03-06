using Domain.Enums;

namespace SocialMedia.Application.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    
    public NotificationType Type { get; set; }

    public string Payload { get; set; } = null!;
    
    public bool IsRead { get; set; }
    
    public DateTime CreatedAt { get; set; }
}