using Domain.Entities;

namespace SocialMedia.Application.Contracts.Repositories;

public interface ITokenRepository
{
    Task<T?> GetValidTokenAsync<T>(string hashedToken, CancellationToken cancellationToken) where T : UserTokenBase;
    
    Task AddAsync<T>(T token, CancellationToken cancellationToken) where T : UserTokenBase;
    
    Task UpdateAsync<T>(T token, CancellationToken cancellationToken) where T : UserTokenBase;
}