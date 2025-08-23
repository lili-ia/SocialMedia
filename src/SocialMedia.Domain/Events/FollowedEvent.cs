namespace Domain.Events;

public class FollowedEvent : NotificationEvent
{
    public Guid FollowerId { get; set; }
    
    public string FollowerUsername { get; set; }
    
    public Guid FolloweeId { get; set; }
}