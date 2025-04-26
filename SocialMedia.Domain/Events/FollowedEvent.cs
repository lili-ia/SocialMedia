namespace Domain.Events;

public class FollowedEvent : NotificationEvent
{
    public int FollowerId { get; set; }
    
    public string FollowerUsername { get; set; }
    
    public int FolloweeId { get; set; }
}