using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class Message : BaseEntity
{
    public Guid? SenderId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = "";

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.Now;

    [Required]
    public MessageType MessageType { get; set; } = MessageType.System;

    public bool IsEdited { get; set; } = false;

    public bool IsRead { get; set; } = false;

    [Required]
    public Guid ChatId { get; set; }

    public virtual Chat? Chat { get; set; }

    public virtual User? Sender { get; set; }
}