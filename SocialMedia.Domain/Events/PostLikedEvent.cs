using System.ComponentModel.DataAnnotations;

namespace Domain.Events;

public class PostLikedEvent : NotificationEvent
{
    [Required]
    public Guid FromUserId { get; set; }
    
    [Required]
    public string FromUsername { get; set; }
    
    [Required]
    public Guid ToUserId { get; set; }
    
    [Required]
    public Guid PostId { get; set; }
}