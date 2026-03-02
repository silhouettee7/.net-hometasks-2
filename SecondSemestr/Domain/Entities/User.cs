using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User
{
    [Column("id")]
    public int Id { get; set; }
    [Column("user_name")]
    public string Username { get; set; } = string.Empty;
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
    public ICollection<EmailMessage>? EmailMessages { get; set; }
}