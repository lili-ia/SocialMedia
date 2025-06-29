using Domain.Enums;

namespace Domain.Entities;

public class Message : BaseEntity
{
    public Guid SenderId { get; set; }

    public string Content { get; set; } = "";

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public MessageType MessageType { get; set; } = Enums.MessageType.System;

    public bool IsEdited { get; set; } = false;

    public bool IsRead { get; set; } = false;

    public Guid ChatId { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User? Sender { get; set; }
}
