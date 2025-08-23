using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public partial class RefreshToken : BaseEntity
{
    [Required]
    [MaxLength(450)] 
    public string Token { get; set; } = null!;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateTime Expires { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public bool IsRevoked { get; set; }

    [MaxLength(45)] 
    public string? IpAddress { get; set; }

    [MaxLength(500)] 
    public string? DeviceInfo { get; set; }

    public virtual User? User { get; set; }
}