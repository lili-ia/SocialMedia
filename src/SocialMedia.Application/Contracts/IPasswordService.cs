namespace SocialMedia.Application.Contracts;

public interface IPasswordService
{
    string HashPassword(string raw);
    bool VerifyPassword(string hashed, string raw);
}