using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SocialMedia.Persistence;

public class SocialMediaDbContextFactory : IDesignTimeDbContextFactory<SocialMediaDbContext>
{
    public SocialMediaDbContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SocialMedia.API"))
            .AddJsonFile("appsettings.json");
        
        IConfiguration config = builder.Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<SocialMediaDbContext>();
        var connectionString = config.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new SocialMediaDbContext(optionsBuilder.Options);
    }
}