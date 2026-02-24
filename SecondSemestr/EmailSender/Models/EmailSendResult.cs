namespace EmailSender.Models;

/// <summary>
/// Результат отправки сообщений пользователям
/// </summary>
public sealed class EmailSendResult
{
    internal EmailSendResult()
    {
        
    }
    internal List<string> SuccessfulEmailsInternal { get; } = new ();
    internal Dictionary<string, string> FailedEmailsInternal { get; } = new ();
    /// <summary>
    /// Почты с удачной отправкой
    /// </summary>
    public IReadOnlyList<string> SuccessfulEmails => SuccessfulEmailsInternal;
    /// <summary>
    /// Почты с ошибками
    /// </summary>
    public IReadOnlyDictionary<string, string> FailedEmails => FailedEmailsInternal;
    /// <summary>
    /// Имеются ли ошибки отправки
    /// </summary>
    public bool HasErrors => FailedEmails.Count > 0;
    /// <summary>
    /// Сколько удачных отправок
    /// </summary>
    public int SuccessCount => SuccessfulEmails.Count;
    /// <summary>
    /// Сколько неудачных отправок
    /// </summary>
    public int FailureCount => FailedEmails.Count;
}