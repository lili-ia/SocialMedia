using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct = default);
    
    Task MarkAllAsReadImmediatelyAsync(Guid userId, CancellationToken ct = default);
    
    Task<List<TResult>> GetAll<TResult>(
        Expression<Func<Notification, bool>> predicate,
        Expression<Func<Notification, TResult>> selector,
        int skip = 0,
        int take = 10,
        CancellationToken ct = default);

    Task RemoveAsync(Expression<Func<Notification, bool>> predicate, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct = default);
}