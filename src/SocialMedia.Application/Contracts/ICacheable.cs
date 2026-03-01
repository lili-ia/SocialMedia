namespace SocialMedia.Application.Contracts;

public interface ICacheable
{
    string CacheKey { get; }
    
    TimeSpan Ttl { get; }
}