using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public sealed class ProfilePic : MediaFile
{
    public string ThumbnailStorageKey { get; private set; } = null!;
    
    public long ThumbnailFileSize { get; private set; }

    private ProfilePic() : base() { }

    private ProfilePic(
        Guid userId, 
        string fileName, 
        ContentType contentType, 
        string storageKey, 
        long fileSizeBytes,
        string thumbnailStorageKey,
        long thumbnailFileSize,
        int? width = null,
        int? height = null) 
        : base(userId, fileName, contentType, storageKey, fileSizeBytes)
    {
        ThumbnailStorageKey = thumbnailStorageKey;
        ThumbnailFileSize = thumbnailFileSize;
        Width = width;
        Height = height;
    }

    public static ProfilePic Create(
        Guid userId, 
        string fileName, 
        ContentType contentType, 
        string storageKey, 
        long fileSizeBytes,
        string thumbnailStorageKey,
        long thumbnailFileSize,
        int? width = null,
        int? height = null)
    {
        if (contentType != ContentType.Image)
        {
            throw new DomainValidationException("Profile pictures must be of type Image.");
        }

        if (string.IsNullOrWhiteSpace(thumbnailStorageKey))
        {
            throw new DomainValidationException("Profile pictures require a thumbnail.");
        }

        return new ProfilePic(
            userId, 
            fileName, 
            contentType, 
            storageKey, 
            fileSizeBytes, 
            thumbnailStorageKey, 
            thumbnailFileSize,
            width, 
            height);
    }
}