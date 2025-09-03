using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SocialMedia.Application.Contracts;

namespace Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_settings.User, _settings.Password)
        };

        var mail = new MailMessage(_settings.FromEmail, to, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mail);
    }
}