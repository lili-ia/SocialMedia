using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface IPendingEmailRepository
{
    Task AddAsync(PendingEmail email, CancellationToken ct);

    Task<List<PendingEmail>> GetUnsentEmailsAsync(CancellationToken ct);
    
    Task<bool> RemoveByIdAsync(Guid id, CancellationToken ct);
}