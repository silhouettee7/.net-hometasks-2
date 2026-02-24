using EmailSender.Abstractions;
using EmailSender.Models;

namespace EmailSender.Impl;

/// <summary>
/// Дефолтный персонализатор, заменяет {{Key}} на Value,
/// Key - свойства конкретного EmailUser
/// Value - значение свойства
/// </summary>
///<remarks>
/// Использует рефлексию, стоит написать свою реализацию
/// </remarks>
public sealed class DefaultEmailMessageTemplatePersonalizer: IEmailMessageTemplatePersonalizer
{
    /// <inheritdoc/>
    public string PersonalizeTemplate<T>(string template, T user) where T : EmailUser
    {
        foreach (var property in user.GetType().GetProperties())
        {
            template = template
                .Replace("{{" + property.Name + "}}", property.GetValue(user)?.ToString());
        }
        
        return template;
    }
}