using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Text;
using EmailSender.Abstractions;
using EmailSender.Models;
using EmailSender.Options;
using Microsoft.Extensions.Logging;

namespace EmailSender;

/// <summary>
/// Сервис для отправки сообщений пользователям
/// </summary>
public sealed class EmailSenderService
{
    private readonly ILogger<EmailSenderService>? _logger;
    private readonly EmailOptions _options;
    
    /// <summary>
    /// Создать сервис
    /// </summary>
    /// <param name="options">конфиг</param>
    public EmailSenderService(EmailOptions options)
    {
        _options = options;
    }
    
    /// <summary>
    /// Создать сервис
    /// </summary>
    /// <param name="options">конфиг</param>
    /// <param name="logger">логгер</param>
    public EmailSenderService(EmailOptions options, ILogger<EmailSenderService> logger)
        : this(options)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Отправить сообщение пользователям
    /// </summary>
    /// <param name="users">список пользователей</param>
    /// <param name="subject">тема сообщения</param>
    /// <param name="message">тело сообщения</param>
    /// <param name="cancellationToken">токен отмены</param>
    /// <typeparam name="T">конкретный тип пользователя</typeparam>
    /// <returns>результат отправки</returns>
    public async Task<EmailSendResult> SendEmailMessageAsync<T>(
        List<T> users,
        string subject,
        string message,
        CancellationToken cancellationToken = default
    ) where T : EmailUser
    {
        ThrowExceptionIfArgumentsAreNull(users, subject, message);
        return await SendEmailAsync(
            users, 
            subject, 
            _ => message, 
            cancellationToken);
    }
    
    /// <summary>
    /// Отправить сообщение пользователям с html шаблоном
    /// </summary>
    /// <param name="users">список пользователей</param>
    /// <param name="subject">тема сообщения</param>
    /// <param name="htmlTemplate">html шаблон тела сообщения</param>
    /// <param name="emailMessageTemplatePersonalizer">персонализатор</param>
    /// <param name="cancellationToken">токен отмены</param>
    /// <typeparam name="T">конкретный тип пользователя</typeparam>
    /// <returns>результат отправки</returns>
    public async Task<EmailSendResult> SendEmailMessageWithTemplateAsync<T>(
        List<T> users, 
        string subject, 
        string htmlTemplate,
        IEmailMessageTemplatePersonalizer emailMessageTemplatePersonalizer,
        CancellationToken cancellationToken = default
        ) where T : EmailUser
    {
        ThrowExceptionIfArgumentsAreNull(users, subject, 
            htmlTemplate, emailMessageTemplatePersonalizer);
        return await SendEmailAsync(
            users, 
            subject, 
            u => emailMessageTemplatePersonalizer
                .PersonalizeTemplate(htmlTemplate, u), 
            cancellationToken);
    }

    private async Task<EmailSendResult> SendEmailAsync<T>(
        List<T> users,
        string subject,
        Func<T, string> createMsg,
        CancellationToken cancellationToken = default
    ) where T : EmailUser
    {
        var result = new EmailSendResult();

        if (users.Count == 0)
        {
            return result;
        }
        
        try
        {
            using var smtpClient = new SmtpClient(_options.SmtpServer, _options.SmtpPort);
            smtpClient.Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword);
            smtpClient.EnableSsl = _options.EnableSsl;
            smtpClient.Timeout = _options.TimeoutMs;
            await SendEmailMessageToUsers(smtpClient, result, users, 
                subject, createMsg, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, ex.Message);
            throw;
        }
        
        return result;
    }

    private async Task SendEmailMessageToUsers<T>(
        SmtpClient smtpClient,
        EmailSendResult result,
        List<T> users,
        string subject,
        Func<T, string> createMsg,
        CancellationToken cancellationToken = default) where T : EmailUser
    {
        for (int i = 0; i < users.Count; i++)
        {
            var user = users[i];
            if (user is null)
            {
                _logger?.LogWarning("пользователь не задан, пропускаем");
                continue;
            }
            if (cancellationToken.IsCancellationRequested)
            {
                _logger?.LogWarning("Отправка прервана. Отправлено {SentCount} из {TotalCount}", 
                    result.SuccessfulEmails.Count, users.Count);
                break;
            }

            try
            {
                string body = createMsg(user);
                
                using var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_options.FromEmail, _options.FromName);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.SubjectEncoding = Encoding.UTF8;

                mailMessage.To.Add(new MailAddress(user.Email));
                
                await smtpClient.SendMailAsync(mailMessage, cancellationToken);
                
                result.SuccessfulEmailsInternal.Add(user.Email);
                _logger?.LogInformation("Письмо отправлено {Email} ({Current}/{Total})", 
                    user.Email, i + 1, users.Count);
                
                if (i < users.Count - 1 && _options.DelayBetweenMessagesMs > 0)
                {
                    await Task.Delay(_options.DelayBetweenMessagesMs, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                result.FailedEmailsInternal[user.Email] = ex.Message;
                _logger?.LogError(ex, "Ошибка отправки {Email}: {Error}", user.Email, ex.Message);

                if (i < users.Count - 1 && _options.DelayBetweenMessagesMs > 0)
                {
                    await Task.Delay(_options.DelayBetweenMessagesMs, cancellationToken);
                }
            }
        }
    }
    private void ThrowExceptionIfArgumentsAreNull(
        IList? users, string? subject, string? body)
    {
        if (_options is null)
        {
            throw new ArgumentNullException(nameof(_options));
        }
        
        if (users is null)
        {
            throw new ArgumentException("Список пользователей не передан", nameof(users));
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Тема не должна быть пустой", nameof(subject));
        }

        if (body is null)
        {
            throw new ArgumentException("Тело должно быть передано");
        }
    }
    
    private void ThrowExceptionIfArgumentsAreNull(
        IList? users, string? subject, string? body, 
        IEmailMessageTemplatePersonalizer emailMessageTemplatePersonalizer)
    {
        ThrowExceptionIfArgumentsAreNull(users, subject, body);
        if (emailMessageTemplatePersonalizer is null)
        {
            throw new ArgumentException(
                "Объект персонализации обязателен, иначе воспользоваться методом SendEmailMessageAsync", 
                nameof(emailMessageTemplatePersonalizer));
        }
    }
}