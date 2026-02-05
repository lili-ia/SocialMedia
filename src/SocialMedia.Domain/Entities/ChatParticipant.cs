namespace Domain.Entities;

public sealed class ChatParticipant : BaseEntity
{
    public Guid ChatId { get; set; }

    public Chat Chat { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
    
    public bool IsAdmin { get; set; }
}