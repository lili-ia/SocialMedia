using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    public NotificationType Type { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime? ReadAt { get; private set; }

    public string Data { get; private set; }

    public Guid RecipientId { get; private set; }
    
    public Guid? ActorId { get; private set; }
    
    public Guid? EntityId { get; private set; }

    public User Recipient { get; private set; } = null!;

    private Notification() { } 

    private Notification(
        NotificationType type,
        string data,
        Guid recipientId,
        Guid? actorId,
        Guid? entityId)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            throw new ArgumentException("Notification data cannot be empty.");
        }

        Type = type;
        Data = data;
        RecipientId = recipientId;
        ActorId = actorId;
        EntityId = entityId;
        IsRead = false;
    }

    public static Notification Create(
        NotificationType type,
        string data,
        Guid recipientId,
        Guid? actorId,
        Guid? entityId)
    {
        return new Notification(type, data, recipientId, actorId, entityId);
    }

    public void MarkAsRead()
    {
        if (IsRead)
        {
            return;
        } 

        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}