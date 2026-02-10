using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Application.Common.Configurations;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Services;

namespace SocialMedia.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddLogging();

        services.AddSingleton<IEmailBuilder, EmailBuilder>();
        
        services.Configure<ClientSettings>(options => 
            config.GetSection(ClientSettings.SectionName).Bind(options));
        
        return services;
    }
}