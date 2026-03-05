namespace Domain.Events;

public class ChatCreatedEvent : DomainEvent
{
    public ChatCreatedEvent(Guid chatId, Guid creatorId, List<Guid> participantIds)
    {
        ChatId = chatId;
        CreatorId = creatorId;
        ParticipantIds = participantIds;
    }

    public Guid ChatId { get; }
    
    public Guid CreatorId { get; }
    
    public List<Guid> ParticipantIds { get; }
}