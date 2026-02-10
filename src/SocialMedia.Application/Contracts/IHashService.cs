namespace SocialMedia.Application.Contracts;

public interface IHashService
{
    string Hash(string raw);
    
    bool Verify(string hashed, string raw);

    string HashDeterministic(string input);
}