using Domain.Enums;

namespace Domain.Entities;

public class PostFile : BaseEntity
{
    public string FileName { set; get; } = null!;

    public ContentType ContentType { get; set; }
    
    public string Url { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    
    public Guid PostId { get; set; }

    public Post Post { get; set; } = null!;
}