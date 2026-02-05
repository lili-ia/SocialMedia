namespace Domain.Entities;

public sealed class MessageAttachment : MediaFile
{
    public Guid MessageId { get; set; }
    
    public Message Message { get; set; } = null!;
}