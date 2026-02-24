namespace EmailSender.Models;

/// <summary>
/// Модель пользователя, которому отправляется сообщение
/// </summary>
/// <remarks>
/// Можно отнаследоваться и добавить кастомные поля 
/// </remarks>
public class EmailUser
{
    /// <summary>
    /// Почта пользователя
    /// </summary>
    public required string Email { get; set; }   
}