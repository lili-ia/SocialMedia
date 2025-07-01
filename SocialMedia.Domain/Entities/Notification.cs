using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class Notification : BaseEntity
{
    [Required]
    public Guid RecipientId { get; set; }

    [Required]
    public NotificationType Type { get; set; } = NotificationType.System;

    public bool IsRead { get; set; } = false;

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.Now;

    [Required]
    public Dictionary<string, string> Data { get; set; } = new();

    public virtual User Recipient { get; set; } = null!;
}