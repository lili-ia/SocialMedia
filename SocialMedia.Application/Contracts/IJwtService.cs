using Domain.Entities;

namespace SocialMedia.Application.Contracts;

public interface IJwtService
{
    public string GenerateToken(string userId, string email);

    RefreshToken GenerateRefreshToken(string? ipAddress = null, string? deviceInfo = null);
}