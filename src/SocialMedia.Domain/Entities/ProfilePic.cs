using Domain.Enums;

namespace Domain.Entities;

public class ProfilePic : BaseEntity
{
    public Guid UserId { get; set; }
    
    public string FileName { get; set; } = null!;
    
    public ContentType ContentType { get; set; }
    
    public string Url { get; set; } = null!;
    
    public User User { get; set; }
}