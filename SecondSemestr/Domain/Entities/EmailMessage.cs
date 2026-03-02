using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class EmailMessage
{
    [Column("id")]
    public int Id { get; set; }
    [Column("subject")]
    public required string Subject { get; set; }
    [Column("body")]
    public required string Body { get; set; } 
    [Column("status")]
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("sent_at")]
    public DateTime? SentAt { get; set; }
    [Column("error_message")]
    public string? ErrorMessage { get; set; }
    [Column("user_id")]
    public int UserId { get; set; }
    
    public User? User { get; set; }
}