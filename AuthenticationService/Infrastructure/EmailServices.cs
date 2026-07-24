using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace AuthenticationService.Infrastructure;

public sealed record EmailMessage(
    string To,
    string Subject,
    string Body,
    bool IsHtml = true);

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
}

public sealed class SmtpEmailSender(
    IOptions<EmailOptions> options,
    ILogger<SmtpEmailSender> logger,
    IHostEnvironment environment) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        if (!_options.IsConfigured)
        {
            if (environment.IsDevelopment())
            {
                logger.LogWarning("Email settings are not configured. Skipping email to {To}. Subject: {Subject}", message.To, message.Subject);
                return;
            }

            throw new InvalidOperationException("Email settings are not configured.");
        }

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };
        mailMessage.To.Add(message.To);

        using var smtpClient = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.UserName, _options.Password)
        };

        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
