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

    public async Task<List<Notification>> GetAll(
        Expression<Func<Notification, bool>> predicate, 
        CancellationToken ct = default)
    {
        return await db.Notifications
            .Where(predicate)
            .ToListAsync(ct);
    }
}