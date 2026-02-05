namespace Domain.Entities;

public sealed class ProfilePic : MediaFile
{
    public string ThumbnailStorageKey { get; set; } = null!;
    
    public long ThumbnailFileSize { get; set; }
}