namespace EmailNotificationsSender.Dtos;

public class EmailStatisticsDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalEmails { get; set; }
    public int SentCount { get; set; }
    public int PendingCount { get; set; }
    public int FailedCount { get; set; }
    public DateTime? LastEmailDate { get; set; }
    public double? AvgSendTimeSeconds { get; set; }
}