namespace Domain.Entities;

public class ChatParticipant
{
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    public Guid ChatId { get; set; }

    public Chat Chat { get; set; } = null!;

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}