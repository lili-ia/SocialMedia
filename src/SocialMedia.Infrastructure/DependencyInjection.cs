using System.Text;
using Amazon.S3;
using Infrastructure.AmazonS3Storage;
using Infrastructure.BackgroundJobs;
using Infrastructure.Caching;
using Infrastructure.Email;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Application.Contracts;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();
        services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));
        services.AddTransient<IEmailSender, SmtpEmailSender>();
        services.AddTransient<IHashService, HashService>();
        services.AddHostedService<EmailBackgroundWorker>();
        services.AddHostedService<TokenBackgroundWorker>();
        AddAuthentication(services, config);
        AddAmazonS3Storage(services, config);
        AddRedis(services, config);
        services.AddScoped<IBlockCacheService, BlockCacheService>();
        
        return services;
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true, 
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ActiveUser", policy => 
                policy.RequireClaim("is_active", "true"));
        });
        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, UserContext>();
    }

    private static void AddAmazonS3Storage(IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(
            config["AmazonS3:AccessKey"],
            config["AmazonS3:SecretKey"],
            Amazon.RegionEndpoint.EUNorth1
        ));

        services.Configure<AmazonS3StorageOptions>(options => 
            config.GetSection(AmazonS3StorageOptions.SectionName).Bind(options));
        
        services.AddSingleton<IFileStorageService, AmazonS3StorageService>();
    }

    private static void AddRedis(IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Redis") ?? "localhost:6379";

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(connectionString));
        
        services.AddSingleton<ICacheService, RedisCacheService>();
    }
}