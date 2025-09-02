using System.Linq.Expressions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SocialMediaDbContext _db;

    public UserRepository(SocialMediaDbContext db)
    {
        _db = db;
    }
    
     public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _db.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User?> GetByEmailOrUsernameAsync(
        string email, 
        string username, 
        CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email || u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid userId, 
        UserRole role = UserRole.User, 
        CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && u.UserRole == role, cancellationToken);
    }

    public async Task<bool> IsActiveAsync(
        Guid userId, 
        UserRole role = UserRole.User, 
        CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId && u.Status == UserStatus.Active && u.UserRole == role, cancellationToken);
    }

    public async Task<Guid?> GetIdByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string?> GetUsernameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => u.Username)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> GetActiveDetailsByIdAsync<TResult>(
        Guid id, 
        Expression<Func<User, TResult>> selector, 
        CancellationToken cancellationToken = default)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id && u.Status == UserStatus.Active)
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TResult>> SearchActiveByUsernameAsync<TResult>(
        string username,
        Expression<Func<User, TResult>> selector,
        List<Guid>? excludeIds = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Users
            .AsNoTracking()
            .Where(u => u.Status == UserStatus.Active && u.Username.Contains(username));

        if (excludeIds != null && excludeIds.Any())
            query = query.Where(u => !excludeIds.Contains(u.Id));

        var results = await query
            .Select(selector)
            .ToListAsync(cancellationToken);

        return results.AsReadOnly();
    }
}