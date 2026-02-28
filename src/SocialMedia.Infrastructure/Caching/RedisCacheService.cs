using System.Text.Json;
using SocialMedia.Application.Contracts;
using StackExchange.Redis;

namespace Infrastructure.Caching;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly IServer _server = redis.GetServer(redis.GetEndPoints().First());

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _db.StringGetAsync(key);

        if (data.IsNull)
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<T>(data!);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new JsonSerializerOptions { WriteIndented = false };
        var json = JsonSerializer.Serialize(value, options);
        
        await _db.StringSetAsync(key, json, expiry ?? TimeSpan.FromMinutes(10));
    }

    public async Task RemoveAsync(string key) => await _db.KeyDeleteAsync(key);
    
    public async Task RemoveByPrefixAsync(string prefix)
    {
        var keys = _server.Keys(pattern: $"{prefix}*").ToArray();

        if (keys.Length > 0)
        {
            await _db.KeyDeleteAsync(keys);
        }
    }
}