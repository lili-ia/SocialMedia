namespace Domain.Entities;

public class Chat : BaseEntity
{
    public string? Title { get; set; }
    
    public bool IsGroup { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid? LastMessageId { get; set; }
    
    public Message? LastMessage { get; set; }

    public ICollection<Message> Messages { get; set; } = [];

    public ICollection<ChatParticipant> ChatParticipants { get; set; } = [];
}