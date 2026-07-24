using ContractMessages.Notifications;
using MassTransit;
using NotificationService.Infrastructure;

namespace NotificationService.Consumers;

public sealed class EmailNotificationRequestedConsumer(
    IEmailSender emailSender,
    ILogger<EmailNotificationRequestedConsumer> logger)
    : IConsumer<IEmailNotificationRequested>
{
    public async Task Consume(ConsumeContext<IEmailNotificationRequested> context)
    {
        var message = context.Message;

        await emailSender.SendAsync(
            new EmailMessage(message.To, message.Subject, message.Body, message.IsHtml),
            context.CancellationToken);

        logger.LogInformation(
            "Email notification {NotificationId} sent to {To}",
            message.NotificationId,
            message.To);
    }
}
