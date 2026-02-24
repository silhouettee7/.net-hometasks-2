using EmailSender.Models;

namespace EmailNotificationsSender.Models;

public class User: EmailUser
{
    public required string Name { get; set; }
}