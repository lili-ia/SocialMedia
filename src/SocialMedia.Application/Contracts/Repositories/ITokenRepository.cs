using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface ITokenRepository
{
    Task<T?> GetValidTokenAsync<T>(string hashedToken, CancellationToken ct) where T : UserTokenBase;
    
    Task AddAsync<T>(T token, CancellationToken ct) where T : UserTokenBase;

    Task RevokeAllUserTokensAsync<T>(Guid userId, CancellationToken ct) where T : UserTokenBase;

    Task<int> RemoveAllRevokedOrExpiredTokensAsync<T>(CancellationToken ct) where T : UserTokenBase;
}