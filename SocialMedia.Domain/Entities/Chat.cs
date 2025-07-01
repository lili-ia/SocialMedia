using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Chat : BaseEntity
{
    [Required]
    public bool IsGroup { get; set; } = false;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = "";

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}