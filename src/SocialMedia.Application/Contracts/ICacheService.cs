namespace SocialMedia.Application.Contracts;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);

    Task RemoveAsync(string key);
    
    Task RemoveByPrefixAsync(string prefix);
}