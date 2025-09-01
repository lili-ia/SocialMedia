namespace SocialMedia.Application.Contracts;

public interface IJwtService
{
    public string GenerateToken(string userId, string email, string role);

    string GenerateRefreshToken();
}