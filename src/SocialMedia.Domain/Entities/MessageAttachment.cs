using Domain.Enums;

namespace Domain.Entities;

public class MessageAttachment : MediaFile
{
    public Guid MessageId { get; private set; }
    
    public Message Message { get; private set; } = null!;

    private MessageAttachment() : base() { }

    private MessageAttachment(
        Guid userId,
        Guid messageId, 
        string fileName, 
        ContentType contentType, 
        string storageKey, 
        long fileSizeBytes) 
        : base(userId, fileName, contentType, storageKey, fileSizeBytes)
    {
        MessageId = messageId;
    }

    public static MessageAttachment Create(
        Guid userId,
        Guid messageId, 
        string fileName,
        ContentType contentType, 
        string storageKey,
        long fileSizeBytes)
    {
        return new MessageAttachment(userId, messageId, fileName, contentType, storageKey, fileSizeBytes);
    }
}