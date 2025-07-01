using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Follow
{
    [Required]
    public Guid FollowerId { get; set; }

    [Required]
    public Guid FolloweeId { get; set; }

    [Required]
    public DateTime FollowedAt { get; set; } = DateTime.Now;

    public virtual User Follower { get; set; } = null!;

    public virtual User Followee { get; set; } = null!;
}