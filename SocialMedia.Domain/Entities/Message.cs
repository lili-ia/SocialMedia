namespace Domain.Entities;

public class Message
{
    public int MessageId { get; set; }

    public int? SenderId { get; set; }

    public string? Content { get; set; }

    public DateTime? Timestamp { get; set; }

    public int? MessageType { get; set; }

    public bool? IsEdited { get; set; }

    public bool? IsRead { get; set; }

    public int? ChatId { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User? Sender { get; set; }
}
