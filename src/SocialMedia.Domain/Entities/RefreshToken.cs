namespace Domain.Entities;

public class RefreshToken : UserTokenBase
{
    public string? IpAddress { get; set; }

    public string? DeviceInfo { get; set; }
}