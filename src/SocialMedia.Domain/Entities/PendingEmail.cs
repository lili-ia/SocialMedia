namespace Domain.Entities;

public class PendingEmail: BaseEntity
{
    public string To { get; set; }
    
    public string Subject { get; set; } 
    
    public string Body { get; set; } 
    
    public int RetryCount { get; set; }
    
    public DateTime? LastAttemptAt { get; set; }

    public bool IsSent { get; set; } = false;
    
    public string? LastError { get; set; }
}