namespace SocialMedia.Application.Contracts;

public interface IUserBlockChecker
{
    Task<bool> IsBlockedAsync(Guid blockerId, Guid blockedId, CancellationToken ct);
    
    Task<List<Guid>> GetUsersBlockedOrBlockingAsync(Guid userId, CancellationToken ct = default);
}