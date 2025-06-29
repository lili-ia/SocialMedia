namespace Domain.Entities;

public class Chat : BaseEntity
{
    public bool IsGroup { get; set; } = false;

    public string Title { get; set; } = "";

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
