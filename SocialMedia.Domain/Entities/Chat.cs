namespace Domain.Entities;

public class Chat
{
    public int ChatId { get; set; }
    public bool? IsGroup { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
