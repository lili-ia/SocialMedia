using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SocialMedia.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<SocialMediaDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"), sqlOptions =>
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10), 
                    errorCodesToAdd: null)
            );
        });
        
        return services;
    }
}