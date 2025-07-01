using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Comment : BaseEntity
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; } = "";

    public Guid? UserId { get; set; }

    [Required]
    public Guid PostId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual Post? Post { get; set; }
    
    public virtual User? User { get; set; }
}