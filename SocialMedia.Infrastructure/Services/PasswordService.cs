using Infrastructure.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private readonly IPasswordHasher<object> _passwordHasher;

    public PasswordService(IPasswordHasher<object> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(string raw)
    {
        return _passwordHasher.HashPassword(null, raw);
    }

    public bool VerifyPassword(string hashed, string raw)
    {
        var result = _passwordHasher.VerifyHashedPassword(null, hashed, raw);
        return result == PasswordVerificationResult.Success;
    }
}