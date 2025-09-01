using Domain.Enums;

namespace Domain.Entities;

public class Message : BaseEntity
{
    public Guid SenderId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public MessageType MessageType { get; set; }

    public bool IsEdited { get; set; }

    public bool IsRead { get; set; }

    public Guid ChatId { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User Sender { get; set; } = null!;
}