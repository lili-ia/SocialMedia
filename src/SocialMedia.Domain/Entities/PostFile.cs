using Domain.Enums;

namespace Domain.Entities;

public class PostFile : BaseEntity
{
    public Guid PostId { get; set; }
    
    public string FileName { get; set; } = null!;
    
    public ContentType ContentType { get; set; }
    
    public string Url { get; set; } = null!;
    
    public Post Post { get; set; }
}