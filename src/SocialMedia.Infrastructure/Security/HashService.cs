using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Security;

public class HashService : IHashService
{
    private readonly IPasswordHasher<object> _passwordHasher;

    public HashService(IPasswordHasher<object> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string Hash(string raw)
    {
        return _passwordHasher.HashPassword(null, raw);
    }

    public bool Verify(string hashed, string raw)
    {
        var result = _passwordHasher.VerifyHashedPassword(null, hashed, raw);
        return result == PasswordVerificationResult.Success;
    }
    
    public string HashDeterministic(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        
        return Convert.ToHexString(bytes); 
    }
}