namespace Domain.Entities;

public sealed class RefreshToken : UserTokenBase
{
    public bool IsUsed { get; set; }
    
    public string? IpAddress { get; set; }
    
    public string? DeviceInfo { get; set; }
    
    public string? ReplacedByToken { get; set; }
}