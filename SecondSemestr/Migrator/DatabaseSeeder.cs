using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Migrator;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var users = CreateUsers();
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        var messages = CreateEmailMessages(users);
        await context.EmailMessages.AddRangeAsync(messages);
        await context.SaveChangesAsync();
    }

    private static List<User> CreateUsers()
    {
        var now = DateTime.UtcNow;
        
        return new List<User>
        {
            new User { Username = "admin", Email = "admin@example.com", CreatedAt = now.AddDays(-30), IsActive = true },
            new User { Username = "john_doe", Email = "john.doe@example.com", CreatedAt = now.AddDays(-25), IsActive = true },
            new User { Username = "jane_smith", Email = "jane.smith@example.com", CreatedAt = now.AddDays(-20), IsActive = true },
            new User { Username = "bob_wilson", Email = "bob.wilson@example.com", CreatedAt = now.AddDays(-15), IsActive = false },
            new User { Username = "alice_brown", Email = "alice.brown@example.com", CreatedAt = now.AddDays(-10), IsActive = true },
            new User { Username = "test_user", Email = "test@example.com", CreatedAt = now.AddDays(-5), IsActive = true }
        };
    }

    private static List<EmailMessage> CreateEmailMessages(List<User> users)
    {
        var messages = new List<EmailMessage>();
        var random = new Random();
        var now = DateTime.UtcNow;

        var subjects = new[]
        {
            "Добро пожаловать в наш сервис!",
            "Важное уведомление о безопасности",
            "Специальное предложение только для вас",
            "Подтверждение регистрации",
            "Еженедельная рассылка новостей",
            "Напоминание о встрече",
            "Счет на оплату",
            "Спасибо за покупку!",
            "Обновление условий использования",
            "Приглашение на вебинар"
        };

        var bodies = new[]
        {
            "Спасибо за регистрацию. Мы рады видеть вас среди наших пользователей.",
            "Обнаружен вход с нового устройства. Если это были не вы, срочно смените пароль.",
            "Только сегодня скидка 30% на все услуги. Успейте воспользоваться!",
            "Пожалуйста, подтвердите свой email, перейдя по ссылке.",
            "Читайте свежие новости и обновления в нашем блоге.",
            "Напоминаем о запланированной встрече завтра в 15:00.",
            "Сумма к оплате. Ожидайте доставку в течение 3 дней.",
            "Ваш заказ оформлен. Ожидайте доставку в течение 3 дней.",
            "Мы обновили условия использования. Пожалуйста, ознакомьтесь.",
            "Приглашаем вас на бесплатный вебинар."
        };

        var errorMessages = new[]
        {
            "SMTP server refused connection",
            "Invalid recipient email address",
            "Mailbox full",
            "Connection timeout",
            "Authentication failed",
            "Daily quota exceeded",
            "Recipient server rejected"
        };

        foreach (var user in users.Where(u => u.IsActive))
        {
            var userMessages = random.Next(3, 8);

            for (int i = 0; i < userMessages; i++)
            {
                var daysAgo = random.Next(1, 30);
                var createdAt = now.AddDays(-daysAgo);
                
                var statusValue = random.Next(100);
                var status = statusValue < 70 ? EmailStatus.Sent : 
                             statusValue < 90 ? EmailStatus.Pending : 
                             EmailStatus.Failed;

                DateTime? sentAt = status == EmailStatus.Sent 
                    ? createdAt.AddMinutes(random.Next(5, 120)) 
                    : null;

                var subject = subjects[random.Next(subjects.Length)];
                var body = bodies[random.Next(bodies.Length)];
                var errorMessage = status == EmailStatus.Failed 
                    ? errorMessages[random.Next(errorMessages.Length)] 
                    : null;

                messages.Add(new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    Status = status,
                    CreatedAt = createdAt,
                    SentAt = sentAt,
                    ErrorMessage = errorMessage,
                    UserId = user.Id
                });
            }
        }

        var admin = users.First(u => u.Username == "admin");
        var testUser = users.First(u => u.Username == "test_user");

        messages.Add(new EmailMessage
        {
            Subject = "Срочное уведомление",
            Body = "Требуется ваше немедленное внимание.",
            Status = EmailStatus.Pending,
            CreatedAt = now.AddMinutes(-5),
            SentAt = null,
            ErrorMessage = null,
            UserId = admin.Id
        });

        messages.Add(new EmailMessage
        {
            Subject = "Важный документ",
            Body = "Документы для подписания во вложении.",
            Status = EmailStatus.Failed,
            CreatedAt = now.AddHours(-2),
            SentAt = null,
            ErrorMessage = "SMTP server timeout after 30 seconds",
            UserId = admin.Id
        });

        messages.Add(new EmailMessage
        {
            Subject = "Архивное сообщение",
            Body = "Это сообщение было отправлено давно.",
            Status = EmailStatus.Sent,
            CreatedAt = now.AddDays(-90),
            SentAt = now.AddDays(-89),
            ErrorMessage = null,
            UserId = testUser.Id
        });

        return messages;
    }
}