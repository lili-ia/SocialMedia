using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class PendingEmailRepository(SocialMediaDbContext db) : IPendingEmailRepository
{
    public async Task AddAsync(PendingEmail email, CancellationToken ct)
    {
        await db.AddAsync(email, ct);
    }

    public async Task<List<PendingEmail>> GetUnsentEmailsAsync(CancellationToken ct)
    {
        return await db.PendingEmails
            .Where(e => !e.IsSent)
            .ToListAsync(ct);
    }

    public async Task<bool> RemoveByIdAsync(Guid id, CancellationToken ct)
    {
        var rowsAffected = await db.PendingEmails
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync(ct);

        return rowsAffected > 0;
    }
}