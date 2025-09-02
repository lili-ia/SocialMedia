using Domain.Enums;

namespace Domain.Entities;

public class ProfilePic : BaseEntity
{
    public string FileName { get; set; } = null!;

    public ContentType ContentType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Url { get; set; } = null!;
    
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}