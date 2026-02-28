using Domain.Enums;

namespace Domain.Entities;

public class EmailMessage
{
    public int Id { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; } 
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    public int UserId { get; set; }
    
    public User? User { get; set; }
}