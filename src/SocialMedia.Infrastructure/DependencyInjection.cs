using System.Text;
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
        return services;
    }
}