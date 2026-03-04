namespace Domain.Events;

public class MessageSentEvent : DomainEvent
{
    public MessageSentEvent(Guid chatId, Guid messageId)
    {
        ChatId = chatId;
        MessageId = messageId;
    }

    public Guid ChatId { get; }
    
    public Guid MessageId { get; }
}