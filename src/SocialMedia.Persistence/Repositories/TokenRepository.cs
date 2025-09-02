using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly SocialMediaDbContext _db;
    
    public TokenRepository(SocialMediaDbContext db)
    {
        _db = db;
    }
    public async Task<T?> GetValidTokenAsync<T>(string hashedToken, CancellationToken cancellationToken) where T : UserTokenBase
    {
        return await _db.Set<T>()
            .FirstOrDefaultAsync(t => 
                    t.Token == hashedToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task AddAsync<T>(T token, CancellationToken cancellationToken) where T : UserTokenBase
    {
        await _db.Set<T>().AddAsync(token, cancellationToken);
    }
}