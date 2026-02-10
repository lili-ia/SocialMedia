using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Email;

public class SmtpEmailSender(IOptions<SmtpSettings> settings, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    private readonly SmtpSettings _settings = settings.Value;

    public async Task<EmailSenderResponse> SendEmailAsync(string to, string subject, string body, CancellationToken ct)
    {
        var message = new MimeMessage();
        
        message.From.Add(new MailboxAddress(_settings.FromUser, _settings.FromEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_settings.Host, _settings.Port, false, ct);
            await client.AuthenticateAsync(_settings.UserName, _settings.Password, ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
            
            return new EmailSenderResponse(IsSuccess: true, ErrorMessage: null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "SMTP error occurred while sending email to {To}", to);
            return new EmailSenderResponse(IsSuccess: false, ErrorMessage: ex.Message);
        }
    }
}