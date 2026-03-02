using EmailNotificationsSender;
using EmailNotificationsSender.Cache;
using EmailNotificationsSender.Models;
using EmailNotificationsSender.Services;
using EmailSender;
using EmailSender.Impl;
using EmailSender.Options;
using Microsoft.Extensions.Configuration;

var cts = new CancellationTokenSource();

using var app = new Application()
    .AddConfiguration()
    .AddConsoleLogging();

if (app.Configuration == null)
{
    Console.WriteLine("Конфигурация не добавлена");
    return;
}

var config = app.Configuration;

while (true)
{
    var help = "Команды:\n" +
               "help - помощь\n" +
               "q - выйти\n" +
               "cache - Запустить службу с выполнением запросов и кешированием\n" +
               "email - Запустить рассылку писем\n";
    Console.WriteLine(help);
    var command = Console.ReadLine()?.ToLower();

    if (command == "q")
    {
        break;
    }

    switch (command)
    {
        case "cache":
            await RunEmailStatisticsService();
            break;
        case "email":
            await RunEmailSending();
            break;
        case "help":
            Console.WriteLine(help);
            break;
        default:
            Console.WriteLine("Команда не найдена");
            break;
    }
}

async Task RunEmailSending()
{
    string emailOptionsSectionName = "EmailOptions";
    
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
    
    var htmlTemplate = File.ReadAllText("HtmlTemplates/htmlTemplateDefault.html");
    
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
                cts.Token);
    
        Console.WriteLine($"Успешно: {result.SuccessCount}");
        Console.WriteLine($"Ошибок: {result.FailureCount}\n");
        
        if (result.HasErrors)
        {
            Console.WriteLine("Ошибки:");
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
}

async Task RunEmailStatisticsService()
{
    var redisConnectionString = config.GetConnectionString("Redis");
    var dbConnectionString = config.GetConnectionString("Db");
    var cache = new RedisService(redisConnectionString ?? "");
    var logger = app.GetLogger<EmailStatisticsService>();
    var service = new EmailStatisticsService(dbConnectionString ?? "", cache, logger);
    while (true)
    {
        Console.WriteLine("\nКоманды:\n" +
                          "q - выйти\n" +
                          "update - Обновить данные в БД\n" +
                          "1 - Выполнить запрос по статистике для юзера\n" +
                          "2 - Выполнить запрос по топ активности\n");
        
        var command = Console.ReadLine()?.ToLower();

        if (command == "q")
        {
            break;
        }
    
        switch (command)
        {
            case "update":
                await service.UpdateDataAsync();
                break;
            case "1":
                await GetUserEmailStatistics(service);
                break;
            case "2":
                await GetTopActiveUsers(service);
                break;
            default:
                Console.WriteLine("Команда не найдена");
                break;
        }
    }
}

async Task GetUserEmailStatistics(EmailStatisticsService service)
{
    Console.WriteLine("Введите user_id");
    var userId = int.Parse(Console.ReadLine());
    var result = await service.GetUserEmailStatistics(userId);
    Console.WriteLine($"username: {result.Username}\n" +
                      $"email: {result.Email}\n" +
                      $"total emails: {result.TotalEmails}");
}

async Task GetTopActiveUsers(EmailStatisticsService service)
{
    Console.WriteLine("Введите limit");
    var limit = int.Parse(Console.ReadLine());
    var result= await service.GetTopActiveUsers(limit);
    foreach (var dto in result)
    {
        Console.WriteLine($"username: {dto.Username}\n" +
                          $"email: {dto.Email}\n" +
                          $"last activity: {dto.LastActivity}");
    }
}
