using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class PostLike
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid PostId { get; set; }

    [Required]
    public DateTime LikedAt { get; set; } = DateTime.Now;

    public virtual User User { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;
}