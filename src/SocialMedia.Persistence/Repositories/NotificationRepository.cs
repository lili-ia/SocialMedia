using System.Linq.Expressions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Application.Contracts.Repositories;

namespace SocialMedia.Persistence.Repositories;

public class NotificationRepository(SocialMediaDbContext db) : INotificationRepository
{
    public async Task AddAsync(Notification notification, CancellationToken ct = default)
    {
        await db.Notifications.AddAsync(notification, ct);
    }

    public async Task MarkAllAsReadImmediatelyAsync(Guid userId, CancellationToken ct = default)
    {
        await db.Notifications
            .Where(n => n.RecipientId == userId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(n => n.IsRead, true), ct);
    }

    public async Task<List<TResult>> GetAll<TResult>(
        Expression<Func<Notification, bool>> predicate, 
        Expression<Func<Notification, TResult>> selector, 
        int skip = 0, 
        int take = 10,
        CancellationToken ct = default)
    {
        return await db.Notifications
            .AsNoTracking()
            .Where(predicate)
            .Select(selector)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task RemoveAsync(Expression<Func<Notification, bool>> predicate, CancellationToken ct = default)
    {
        var notifications = await db.Notifications
            .Where(predicate)
            .ToListAsync(ct);

        foreach (var n in notifications)
        {
            n.SoftDelete();
        }
    }

    public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct = default)
    {
        await db.AddRangeAsync(notifications, ct);
    }
}