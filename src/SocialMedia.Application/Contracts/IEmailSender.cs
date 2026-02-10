namespace SocialMedia.Application.Contracts;

public interface IEmailSender
{
    Task<EmailSenderResponse> SendEmailAsync(string email, string subject, string message, CancellationToken ct);
}