namespace Domain.Events;

public class ChatParticipantAddedEvent : DomainEvent
{
    public Guid ChatId { get; }
    
    public Guid UserId { get; }

    public ChatParticipantAddedEvent(Guid chatId, Guid userId)
    {
        ChatId = chatId;
        UserId = userId;
    }
}