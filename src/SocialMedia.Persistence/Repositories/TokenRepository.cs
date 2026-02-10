using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class TokenRepository(SocialMediaDbContext db) : ITokenRepository
{
    public async Task<T?> GetValidTokenAsync<T>(string hashedToken, CancellationToken ct) where T : UserTokenBase
    {
        return await db.Set<T>()
            .FirstOrDefaultAsync(t => t.Token == hashedToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow, ct);
    }

    public async Task AddAsync<T>(T token, CancellationToken ct) where T : UserTokenBase
    {
        await db.Set<T>().AddAsync(token, ct);
    }

    public async Task RevokeAllUserTokensAsync<T>(Guid userId, CancellationToken ct) where T : UserTokenBase
    {
        await db.Set<T>()
            .Where(t => t.UserId == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(t => t.IsRevoked, true)
                .SetProperty(t => t.RevokedAt, DateTime.UtcNow)
                .SetProperty(t => t.ReasonForRevocation, "Refresh Token Replay Detected"), ct);
    }

    public async Task<int> RemoveAllRevokedOrExpiredTokensAsync<T>(CancellationToken ct) where T : UserTokenBase
    {
        return await db.Set<T>()
            .Where(t => t.IsRevoked || t.IsExpired)
            .ExecuteDeleteAsync(ct);
    }
}