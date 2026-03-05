using Domain.Enums;

namespace Domain.Entities;

public abstract class MediaFile : BaseEntity
{
    public Guid UserId { get; protected set; }
    
    public User User { get; protected set; } = null!;
    
    public string FileName { get; protected set; } = null!;
    
    public ContentType ContentType { get; protected set; }
    
    public string StorageKey { get; protected set; } = null!;
    
    public long FileSizeBytes { get; protected set; }
    
    public int? Width { get; protected set; }
    
    public int? Height { get; protected set; }

    protected MediaFile() { }

    protected MediaFile(Guid userId, string fileName, ContentType contentType, string storageKey, long fileSizeBytes)
    {
        UserId = userId;
        FileName = fileName;
        ContentType = contentType;
        StorageKey = storageKey;
        FileSizeBytes = fileSizeBytes;
    }
}