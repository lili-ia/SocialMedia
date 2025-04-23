using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }   
    
    public string GenerateToken(string userId, string email)
    {
        var claims = new[]
        {       
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims, 
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken(string? ipAddress = null, string? deviceInfo = null)
    {
        var refreshToken = new RefreshToken
        {
            Token = GenerateRandomToken(),
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo
        };

        return refreshToken;
    }
    
    private string GenerateRandomToken()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[64];  // 64 bytes for a 512-bit token
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}