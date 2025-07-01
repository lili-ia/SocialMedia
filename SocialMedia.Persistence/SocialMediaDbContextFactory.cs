using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SocialMedia.Persistence;

public class SocialMediaDbContextFactory : IDesignTimeDbContextFactory<SocialMediaContext>
{
    public SocialMediaContext CreateDbContext(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "SocialMedia.Presentation"))
            .AddJsonFile("appsettings.json");
        
        IConfiguration config = builder.Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<SocialMediaContext>();
        var connectionString = config.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");

        optionsBuilder.UseSqlServer(connectionString);

        return new SocialMediaContext(optionsBuilder.Options);
    }
}