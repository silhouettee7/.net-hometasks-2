namespace EmailSender.Options;

/// <summary>
/// Конфиг для отправки почтовых сообщений
/// </summary>
public sealed class EmailOptions
{
    /// <summary>
    /// Почтовый сервер
    /// </summary>
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    /// <summary>
    /// Порт
    /// </summary>
    public int SmtpPort { get; set; } = 587;
    /// <summary>
    /// ssl вкл/выкл
    /// </summary>
    public bool EnableSsl { get; set; } = true;
    /// <summary>
    /// Логин от почты
    /// </summary>
    public required string SmtpUsername { get; set; }
    /// <summary>
    /// Пароль от почты
    /// </summary>
    public required string SmtpPassword { get; set; }
    /// <summary>
    /// С какой почты отправить
    /// </summary>
    public required string FromEmail { get; set; }
    /// <summary>
    /// От какого имени отправить
    /// </summary>
    public required string FromName { get; set; }
    /// <summary>
    /// Задержка между отправками сообщений
    /// </summary>
    public int DelayBetweenMessagesMs { get; set; } = 1000;
    /// <summary>
    /// Таймаут ожидания отправки
    /// </summary>
    public int TimeoutMs { get; set; } = 10000;
}