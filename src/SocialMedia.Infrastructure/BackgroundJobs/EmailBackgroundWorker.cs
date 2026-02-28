using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Contracts;
using SocialMedia.Application.Contracts.Repositories;

namespace Infrastructure.BackgroundJobs;

public class EmailBackgroundWorker(
    IServiceProvider serviceProvider,
    ILogger<EmailBackgroundWorker> logger) : BackgroundService
{
    private const int MaxRetries = 5; 
    private readonly TimeSpan _period = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Email Background Worker is starting.");

        using PeriodicTimer timer = new(_period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessPendingEmailsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing pending emails.");
            }
        }
    }

    private async Task ProcessPendingEmailsAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        
        var emailRepository = scope.ServiceProvider.GetRequiredService<IPendingEmailRepository>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var pendingEmails = await emailRepository.GetUnsentEmailsAsync(ct);

        if (!pendingEmails.Any())
        {
            return;
        }

        logger.LogInformation("Processing {Count} pending emails...", pendingEmails.Count);

        foreach (var email in pendingEmails)
        {
            var result = await emailSender.SendEmailAsync(email.To, email.Subject, email.Body, ct);

            if (result.IsSuccess)
            {
                email.MarkAsSent();
                logger.LogInformation("Successfully sent pending email to {To}", email.To);
            }
            else
            {
                email.MarkAsFailed(result.ErrorMessage);

                if (email.CanRetry(MaxRetries))
                {
                    continue;
                }
                
                logger.LogError("Email to {To} failed after 5 attempts. Giving up.", email.To);
            }

            await emailRepository.RemoveByIdAsync(email.Id, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}