using EmailSender.Models;

namespace EmailSender.Abstractions;

/// <summary>
/// Абстракция для генерации почтового сообщения по HTML шаблону для конкретного пользователя
/// </summary>
public interface IEmailMessageTemplatePersonalizer
{
    /// <summary>
    /// Персонализирует HTML шаблон под конкретного пользователя
    /// </summary>
    /// <param name="template">HTML шаблон</param>
    /// <param name="user">пользователь</param>
    /// <typeparam name="T">конкретный тип пользователя</typeparam>
    /// <returns>email сообщение</returns>
    string PersonalizeTemplate<T>(string template, T user) where T : EmailUser;
}