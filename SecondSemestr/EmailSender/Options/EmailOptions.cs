namespace EmailSender.Options;

public class EmailOptions
{
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public int DelayBetweenMessagesMs { get; set; } = 1000;
    public int TimeoutMs { get; set; } = 10000;
}