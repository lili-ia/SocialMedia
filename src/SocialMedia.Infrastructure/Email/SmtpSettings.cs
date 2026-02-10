namespace Infrastructure.Email;

public class SmtpSettings
{
    public string Host { get; set; } = null!;
    
    public int Port { get; set; }
    
    public string FromUser { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public string FromEmail { get; set; } = null!;
    
    public string UserName { get; set; } = null!;
}