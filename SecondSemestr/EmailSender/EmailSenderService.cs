using System.Net;
using System.Net.Mail;
using EmailSender.Models;
using EmailSender.Options;

namespace EmailSender;

public class EmailSenderService(EmailOptions options)
{
    public async Task SendEmailAsync<T>(
        List<T> users, 
        string subject, 
        string htmlTemplate,
        CancellationToken cancellationToken = default
        ) where T : EmailUser
    {
        var result = new EmailSendResult();
        
        using var smtpClient = new SmtpClient(options.SmtpServer, options.SmtpPort);
        smtpClient.Credentials = new NetworkCredential(options.SmtpUsername, options.SmtpPassword);
        smtpClient.EnableSsl = options.EnableSsl;
        smtpClient.Timeout = options.TimeoutMs;
        
    }
}