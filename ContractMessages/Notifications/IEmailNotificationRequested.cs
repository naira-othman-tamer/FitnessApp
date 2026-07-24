namespace ContractMessages.Notifications;

public interface IEmailNotificationRequested
{
    Guid NotificationId { get; }
    string To { get; }
    string Subject { get; }
    string Body { get; }
    bool IsHtml { get; }
    DateTime RequestedAtUtc { get; }
}
