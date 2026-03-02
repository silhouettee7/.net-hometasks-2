namespace EmailNotificationsSender.Services;

public class UserEmailSummaryDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int EmailCount { get; set; }
    public int SentCount { get; set; }
    public DateTime? LastActivity { get; set; }
}