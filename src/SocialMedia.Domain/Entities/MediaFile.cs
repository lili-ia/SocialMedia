using Domain.Enums;

namespace Domain.Entities;

public class MediaFile : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    
    public string OriginalFileName { get; set; } = null!;
    
    public ContentType ContentType { get; set; }
    
    public string OriginalStorageKey { get; set; } = null!;
    
    public long OriginalFileSize { get; set; }
    
    public int? Width { get; set; }
    
    public int? Height { get; set; }
}