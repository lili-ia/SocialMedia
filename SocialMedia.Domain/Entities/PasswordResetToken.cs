using System.ComponentModel.DataAnnotations;
using SocialMedia.Application.Attributes;

namespace Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MinLength(20, ErrorMessage = "Token must be greater than 20 symbols")]
    public string Token { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    [DateGreaterThan("CreatedAt", ErrorMessage = "ExpiresAt must be later than CreatedAt")]
    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    public virtual User User { get; set; }
}