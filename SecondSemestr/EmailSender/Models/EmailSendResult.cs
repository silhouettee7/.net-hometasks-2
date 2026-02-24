namespace EmailSender.Models;

internal class EmailSendResult
{
    internal List<string> SuccessfulEmailsInternal { get; } = new ();
    internal Dictionary<string, string> FailedEmailsInternal { get; } = new ();
    public IReadOnlyList<string> SuccessfulEmails => SuccessfulEmailsInternal;
    public IReadOnlyDictionary<string, string> FailedEmails => FailedEmailsInternal;
    public bool HasErrors => FailedEmails.Count > 0;
    public int SuccessCount => SuccessfulEmails.Count;
    public int FailureCount => FailedEmails.Count;
}