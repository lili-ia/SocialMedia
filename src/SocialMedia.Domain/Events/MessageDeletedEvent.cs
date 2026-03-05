namespace Domain.Events;

public class MessageDeletedEvent : DomainEvent
{
    public MessageDeletedEvent(Guid chatId, Guid messageId, Guid senderId)
    {
        ChatId = chatId;
        MessageId = messageId;
        SenderId = senderId;
    }

    public Guid ChatId { get; }
    
    public Guid MessageId { get; }
    
    public Guid SenderId { get; }
}