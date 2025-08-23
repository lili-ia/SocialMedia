using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Post : BaseEntity
{
    [StringLength(2000)]
    public string? Text { get; set; } = "";

    [Required]
    public Guid UserId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [Required]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
}