using System.Linq.Expressions;
using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct = default);

    Task<List<Notification>> GetAll(
        Expression<Func<Notification, bool>> predicate,
        CancellationToken ct = default);
}