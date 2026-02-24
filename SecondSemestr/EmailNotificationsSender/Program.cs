using EmailNotificationsSender;
using EmailNotificationsSender.Models;
using EmailSender;
using EmailSender.Impl;
using EmailSender.Options;
using Microsoft.Extensions.Configuration;

var cts = new CancellationTokenSource();
var token = cts.Token;

var app = new Application()
    .AddConfiguration()
    .AddConsoleLogging();

if (app.Configuration == null)
{
    Console.WriteLine("Конфигурация не добавлена");
    return;
}

string emailOptionsSectionName = "EmailOptions";
var config = app.Configuration;

var options = config
    .GetSection(emailOptionsSectionName)
    .Get<EmailOptions>();

if (options == null)
{
    Console.WriteLine($"Секции {emailOptionsSectionName} не существует");
    return;
}

var logger = app.GetLogger<EmailSenderService>();

var emailSenderService = new EmailSenderService(options, logger);

var htmlTemplate = File.ReadAllText("htmlTemplateDefault.html");

var users = new List<User>
{
    config.GetSection("User1").Get<User>(),
    config.GetSection("User2").Get<User>()
};

try
{
    var result = await emailSenderService
        .SendEmailMessageWithTemplateAsync(
            users,
            "Сообщение",
            htmlTemplate,
            new DefaultEmailMessageTemplatePersonalizer(),
            token);

    Console.WriteLine($"Успешно: {result.SuccessCount}");
    Console.WriteLine($"Ошибок: {result.FailureCount}\n");
    
    if (result.HasErrors)
    {
        Console.WriteLine("Ошибки:\n");
        foreach (var error in result.FailedEmails)
        {
            Console.WriteLine($"Email - {error.Key}:\n{error.Value}\n");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
