namespace Infrastructure.Email;

public class SmtpSettings
{
    public string Host { get; set; } = null!;
    
    public int Port { get; set; }
    
    public string User { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public string FromEmail { get; set; } = null!;
    
    public string FromName { get; set; } = null!;
}