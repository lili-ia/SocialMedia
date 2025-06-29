namespace Domain.Entities;

public partial class RefreshToken : BaseEntity
{
    public string Token { get; set; } = null!;

    public Guid UserId { get; set; }

    public DateTime Expires { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsRevoked { get; set; }

    public string? IpAddress { get; set; }

    public string? DeviceInfo { get; set; }
    
    public virtual User? User { get; set; }
}
