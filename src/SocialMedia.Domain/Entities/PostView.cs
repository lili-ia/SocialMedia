namespace Domain.Entities;

public sealed class PostView : BaseEntity // note only insert if no view exists for last 6 hours
{
    public Guid UserId { get; set; } 

    public Guid PostId { get; set; }

    public User User { get; set; } = null!;
    
    public Post Post { get; set; } = null!;
}