using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Security;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateAccessToken(string userId, string email, string role, bool isActive)
    {
        var claims = new[]
        {       
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("is_active", isActive.ToString().ToLower())
        };
        
        var keyString = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(keyString))
            throw new InvalidOperationException("JWT Key is not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),  
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return GenerateRandomToken();
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
            
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
            
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero 
            }, out var _);

            return principal;
        }
        catch (Exception ex)
        {
            return null; 
        }
    }

    private string GenerateRandomToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[64]; 
        rng.GetBytes(bytes);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}