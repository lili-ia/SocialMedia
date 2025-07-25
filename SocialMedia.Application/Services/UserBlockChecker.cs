using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts;
using SocialMedia.Persistence;

namespace SocialMedia.Application.Services;

public class UserBlockChecker : IUserBlockChecker
{
    private readonly SocialMediaContext _db;
    
    public UserBlockChecker(SocialMediaContext db)
    {
        _db = db;
    }
    
    public async Task<bool> IsBlockedAsync(Guid blockerId, Guid blockedId, CancellationToken ct)
    {
        return await _db.Blocks
            .AnyAsync(u => u.BlockerId == blockerId && u.BlockedId == blockedId, ct);
    }

    public async Task<List<Guid>> GetUsersBlockedOrBlockingAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.Blocks
            .Where(b => b.BlockerId == userId || b.BlockedId == userId)
            .Select(b => b.BlockerId == userId ? b.BlockedId : b.BlockerId)
            .Distinct()
            .ToListAsync(ct);
    }
}