using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace AuthenticationService.Infrastructure;

public interface IOtpNotificationService
{
    Task SendPasswordResetOtpAsync(string email, string otp, CancellationToken cancellationToken);
}

public sealed class OtpEmailNotificationService(
    IEmailSender emailSender,
    ILogger<OtpEmailNotificationService> logger,
    IHostEnvironment environment)
    : IOtpNotificationService
{
    public async Task SendPasswordResetOtpAsync(string email, string otp, CancellationToken cancellationToken)
    {
        if (environment.IsDevelopment())
            logger.LogInformation("Development password reset OTP for {Email}: {Otp}", email, otp);

        var body = $"""
                   <p>Hello,</p>
                   <p>Your password reset OTP is:</p>
                   <h2>{otp}</h2>
                   <p>This code expires in 10 minutes.</p>
                   <p>If you did not request this reset, please ignore this email.</p>
                   """;

        await emailSender.SendAsync(
            new EmailMessage(email, "Fitness App Password Reset OTP", body),
            cancellationToken);

        logger.LogInformation("Password reset OTP email sent to {Email}", email);
    }
}

public interface IProfileLifecyclePublisher
{
    Task PublishProfileLifecycleInitiatedAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed class LoggingProfileLifecyclePublisher(ILogger<LoggingProfileLifecyclePublisher> logger)
    : IProfileLifecyclePublisher
{
    public Task PublishProfileLifecycleInitiatedAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Profile lifecycle initiated for user {UserId}", userId);
        return Task.CompletedTask;
    }
}

public sealed record AccountState(bool ProfileCompleted, bool IsPremium);

public interface IAccountStateCache
{
    Task<AccountState> GetAsync(Guid userId, CancellationToken cancellationToken);
    Task MarkProfileCompletedAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed class AccountStateCache(IDistributedCache cache) : IAccountStateCache
{
    private static string Key(Guid userId) => $"auth:account-state:{userId:N}";

    public async Task<AccountState> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        var json = await cache.GetStringAsync(Key(userId), cancellationToken);
        return json is null ? new AccountState(false, false) : JsonSerializer.Deserialize<AccountState>(json)!;
    }

    public async Task MarkProfileCompletedAsync(Guid userId, CancellationToken cancellationToken)
    {
        var state = await GetAsync(userId, cancellationToken);
        await cache.SetStringAsync(Key(userId), JsonSerializer.Serialize(state with { ProfileCompleted = true }),
            cancellationToken);
    }
}

public interface IAccessTokenRevocationStore
{
    Task RevokeAsync(string jti, DateTimeOffset expiresAt, CancellationToken cancellationToken);
    Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken);
}

public sealed class AccessTokenRevocationStore(IDistributedCache cache) : IAccessTokenRevocationStore
{
    private static string Key(string jti) => $"auth:revoked:{jti}";

    public Task RevokeAsync(string jti, DateTimeOffset expiresAt, CancellationToken cancellationToken) =>
        cache.SetStringAsync(Key(jti), "1", new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = expiresAt
        }, cancellationToken);

    public async Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken) =>
        await cache.GetStringAsync(Key(jti), cancellationToken) is not null;
}
