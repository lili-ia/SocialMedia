using System.Text;
using Azure.Storage.Blobs;
using Infrastructure.AzureBlobStorage;
using Infrastructure.BackgroundJobs;
using Infrastructure.Email;
using Infrastructure.Security;
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
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();
        services.Configure<SmtpSettings>(config.GetSection("SmtpSettings"));
        services.AddTransient<IEmailSender, SmtpEmailSender>();
        services.AddTransient<IHashService, HashService>();
        services.AddHostedService<EmailBackgroundWorker>();
        services.AddHostedService<TokenBackgroundWorker>();
        AddAuthentication(services, config);
        AddAzureStorage(services, config);
        
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
        services.Configure<AzureStorageOptions>(options => 
            config.GetSection(AzureStorageOptions.SectionName).Bind(options));
        
        services.AddSingleton(x =>
        {
            var options = config.GetSection("AzureStorage").Get<AzureStorageOptions>();
            return new BlobServiceClient(options.ConnectionString);
        });
        services.AddSingleton<IFileStorageService, AzureBlobStorageService>();
    }
}