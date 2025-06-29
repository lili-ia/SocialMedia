namespace Domain.Entities;

public class Comment : BaseEntity
{
    public string Text { get; set; } = "";

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public DateTime CreatedAt { get; set; } =  DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public virtual Post? Post { get; set; }

    public virtual User? User { get; set; }
}
