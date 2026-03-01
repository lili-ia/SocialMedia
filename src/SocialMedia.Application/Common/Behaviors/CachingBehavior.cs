using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;

namespace SocialMedia.Application.Common.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not ICacheable cacheable)
        {
            return await next();
        }

        var cacheKey = cacheable.CacheKey;

        var cached = await cacheService.GetAsync<TResponse>(cacheKey);
        
        if (cached is not null)
        {
            logger.LogInformation("Cache hit for {CacheKey}.", cacheKey);
            return cached;
        }

        logger.LogInformation("Cache miss for {CacheKey}. Fetching from handler.", cacheKey);

        var response = await next();

        await cacheService.SetAsync(cacheKey, response, cacheable.Ttl, ct);

        return response;
    }
}