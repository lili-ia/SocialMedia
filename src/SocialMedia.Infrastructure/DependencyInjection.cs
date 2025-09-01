using System.Text;
using Azure.Storage.Blobs;
using Infrastructure.AzureBlobStorage;
using Infrastructure.Email;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Application.Contracts;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();
        services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));
        services.AddTransient<IEmailSender, SmtpEmailSender>();
        services.AddTransient<IPasswordService, PasswordService>();
        AddAuthentication(services, config);
        
        
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
        
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddTransient<IUserContext, UserContext>();
    }

    private static void AddAzureStorage(IServiceCollection services, IConfiguration config)
    {
        services.Configure<AzureStorageOptions>(config.GetSection("AzureStorage"));
        services.AddSingleton(x => new BlobServiceClient(config
            .GetConnectionString("AzureStorage:ConnectionString")));
        services.AddSingleton<IFileStorageService, AzureBlobStorageService>();
    }
}