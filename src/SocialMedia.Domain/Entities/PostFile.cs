using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public sealed class PostFile : MediaFile
{
    public Guid PostId { get; private set; }

    public Post Post { get; private set; } = null!;

    private PostFile() : base() { }

    private PostFile(
        Guid userId, 
        Guid postId, 
        string fileName, 
        ContentType contentType, 
        string storageKey, 
        long fileSizeBytes,
        int? width = null,
        int? height = null) 
        : base(userId, fileName, contentType, storageKey, fileSizeBytes)
    {
        PostId = postId;
        Width = width;
        Height = height;
    }

    public static PostFile Create(
        Guid userId, 
        Guid postId, 
        string fileName, 
        ContentType contentType, 
        string storageKey, 
        long fileSizeBytes,
        int? width = null,
        int? height = null)
    {
        if (postId == Guid.Empty)
        {
            throw new DomainValidationException("PostId is required.");
        }

        if (string.IsNullOrWhiteSpace(storageKey))
        {
            throw new DomainValidationException("Storage key is required.");
        }

        return new PostFile(
            userId, 
            postId, 
            fileName, 
            contentType, 
            storageKey, 
            fileSizeBytes, 
            width, 
            height);
    }
}