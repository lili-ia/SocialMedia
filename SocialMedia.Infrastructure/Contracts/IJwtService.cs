namespace Infrastructure.Contracts;

public interface IJwtService
{
    public string GenerateToken(string userId, string email);
}