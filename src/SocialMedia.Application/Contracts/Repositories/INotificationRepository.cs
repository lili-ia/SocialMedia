using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken ct = default);
}