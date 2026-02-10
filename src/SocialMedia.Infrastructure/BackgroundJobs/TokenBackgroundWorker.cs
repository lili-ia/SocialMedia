using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts.Repositories;

namespace Infrastructure.BackgroundJobs;

public class TokenBackgroundWorker( 
    IServiceProvider serviceProvider,
    ILogger<TokenBackgroundWorker> logger) : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromDays(1);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Token Background Worker is starting.");

        using PeriodicTimer timer = new(_period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await CleanupTokensAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing expired or revoked tokens.");
            }
        }
    }

    private async Task CleanupTokensAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        
        var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();

        var rows = await tokenRepository.RemoveAllRevokedOrExpiredTokensAsync<RefreshToken>(ct);
        rows += await tokenRepository.RemoveAllRevokedOrExpiredTokensAsync<EmailConfirmationToken>(ct);
        rows += await tokenRepository.RemoveAllRevokedOrExpiredTokensAsync<PasswordResetToken>(ct);

        logger.LogInformation("Successfully removed {Count} outdated tokens.", rows);
    }
}