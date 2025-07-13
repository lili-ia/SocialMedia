using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class PostView : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid PostId { get; set; }
    
    [Required]
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User User { get; set; } = null!;
    
    public virtual Post Post { get; set; } = null!;
}