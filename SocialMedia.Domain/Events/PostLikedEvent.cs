using System.ComponentModel.DataAnnotations;

namespace Domain.Events;

public class PostLikedEvent : NotificationEvent
{
    [Required]
    public int FromUserId { get; set; }
    
    [Required]
    public string FromUsername { get; set; }
    
    [Required]
    public int ToUserId { get; set; }
    
    [Required]
    public int PostId { get; set; }
}