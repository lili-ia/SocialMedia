using Domain.Exceptions;

namespace Domain.Entities;

public sealed class PendingEmail : BaseEntity
{
    public string To { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }

    public int RetryCount { get; private set; }

    public DateTime? LastAttemptAt { get; private set; }

    public bool IsSent { get; private set; }

    public string? LastError { get; private set; }

    private PendingEmail() { }

    private PendingEmail(
        string to,
        string subject,
        string body)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            throw new DomainValidationException("Recipient email is required.");
        }

        if (string.IsNullOrWhiteSpace(subject))
            throw new DomainValidationException("Subject is required.");

        To = to;
        Subject = subject;
        Body = body;
        IsSent = false;
        RetryCount = 0;
    }

    public static PendingEmail Create(
        string to,
        string subject,
        string body)
    {
        return new PendingEmail(to, subject, body);
    }

    public void MarkAsSent()
    {
        IsSent = true;
        LastAttemptAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string? error)
    {
        RetryCount++;
        LastAttemptAt = DateTime.UtcNow;
        LastError = error;
    }

    public bool CanRetry(int maxRetries = 5)
    {
        return !IsSent && RetryCount < maxRetries;
    }
}