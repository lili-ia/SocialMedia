namespace Domain.Events;

public class MessageSentEvent : DomainEvent
{
    public MessageSentEvent(Guid chatId, Guid messageId, Guid senderId, string? text, int? attachmentsCount)
    {
        ChatId = chatId;
        MessageId = messageId;
        SenderId = senderId;
        Text = text;
        AttachmentsCount = attachmentsCount;
    }

    public Guid ChatId { get; }
    
    public Guid MessageId { get; }
    
    public Guid SenderId { get; }
    
    public string? Text { get; }
    
    public int? AttachmentsCount { get; }
}