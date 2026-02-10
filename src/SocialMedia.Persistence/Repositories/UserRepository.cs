using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class UserRepository(SocialMediaDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default, bool tracking = false)
    {
        var query = db.Users.AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await db.Users.AddAsync(user, ct);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(
        string email, 
        string username, 
        CancellationToken ct = default,
        bool tracking = false)
    {
        var query = db.Users.AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(u => u.EmailNormalized == email || u.UsernameNormalized == username, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default, bool tracking = false)
    {
        var query = db.Users.AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }
        
        return await query.FirstOrDefaultAsync(u => u.EmailNormalized == email, ct);
    }

    public async Task<bool> ExistsAsync(
        Guid userId, 
        UserRole role = UserRole.User, 
        CancellationToken ct = default)
    {
        return await db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && u.UserRole == role, ct);
    }

    public async Task<bool> IsActiveAsync(
        Guid userId, 
        UserRole role = UserRole.User, 
        CancellationToken ct = default)
    {
        return await db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && u.Status == UserStatus.Active && u.UserRole == role, ct);
    }

    public async Task<Guid?> GetIdByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.UsernameNormalized == username)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<string?> GetUsernameByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => u.UsernameNormalized)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<TResult?> GetActiveDetailsByIdAsync<TResult>(
        Guid id, 
        Expression<Func<User, TResult>> selector, 
        CancellationToken ct = default)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Id == id && u.Status == UserStatus.Active)
            .Select(selector)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<TResult>> SearchActiveByUsernameAsync<TResult>(
        string username,
        Expression<Func<User, TResult>> selector,
        List<Guid>? excludeIds = null,
        CancellationToken ct = default)
    {
        var query = db.Users
            .AsNoTracking()
            .Where(u => u.Status == UserStatus.Active && u.UsernameNormalized.Contains(username));

        if (excludeIds != null && excludeIds.Any())
            query = query.Where(u => !excludeIds.Contains(u.Id));

        var results = await query
            .Select(selector)
            .ToListAsync(ct);

        return results.AsReadOnly();
    }
}