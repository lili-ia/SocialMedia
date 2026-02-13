using System.Security.Claims;

namespace SocialMedia.Application.Contracts;

public interface ITokenService
{
    public string GenerateAccessToken(string userId, string email, string role, bool isActive);

    string GenerateRefreshToken();

    ClaimsPrincipal? ValidateToken(string token);
}