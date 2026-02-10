using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Persistence.Repositories;

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
        
        AddRepositories(services);
        
        return services;
    }
    
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IBlockRepository, BlockRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IPostLikeRepository, PostLikeRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<IPendingEmailRepository, PendingEmailRepository>(); 
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}