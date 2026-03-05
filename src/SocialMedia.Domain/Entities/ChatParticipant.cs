namespace Domain.Entities;

public class ChatParticipant : BaseEntity
{
    public Guid ChatId { get; private set; }
    public Guid UserId { get; private set; }
    public bool IsAdmin { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastReadAt { get; private set; }

    public Chat Chat { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private ChatParticipant() { }

    public static ChatParticipant Create(Guid chatId, Guid userId, bool isAdmin)
        => new()
        {
            ChatId = chatId,
            UserId = userId,
            IsAdmin = isAdmin,
        };

    public void Deactivate() => IsActive = false;

    public void MarkAsRead() => LastReadAt = DateTime.UtcNow;
}