using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Infrastructure;
using ContractMessages.Notifications;
using MassTransit;
using MediatR;

namespace AuthenticationService.Features.Auth.CompleteProfile;

public sealed record CompleteProfileCommand(Guid UserId, string? Email) : IRequest<OperationResult>;

public sealed class CompleteProfileHandler(
    IProfileLifecyclePublisher publisher,
    IAccountStateCache cache,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<CompleteProfileCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
    {
        await publisher.PublishProfileLifecycleInitiatedAsync(request.UserId, cancellationToken);
        await cache.MarkProfileCompletedAsync(request.UserId, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            await publishEndpoint.Publish<IEmailNotificationRequested>(new
            {
                NotificationId = Guid.NewGuid(),
                To = request.Email,
                Subject = "Welcome to Fitness App",
                Body = """
                       <p>Welcome to Fitness App.</p>
                       <p>Your profile is now complete, and your fitness journey is ready to start.</p>
                       """,
                IsHtml = true,
                RequestedAtUtc = DateTime.UtcNow
            }, cancellationToken);
        }

        return OperationResultFactory.Success(
            message: "Profile lifecycle initiated.",
            messageLocalized: "Profile lifecycle initiated.");
    }
}

public static class CompleteProfileEndpoint
{
    public static RouteGroupBuilder MapCompleteProfile(this RouteGroupBuilder group)
    {
        group.MapPost("/complete-profile", async (ClaimsPrincipal principal, ISender sender, CancellationToken ct) =>
        {
            if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                return OperationResultFactory.UnAuthorized().ToHttpResult();

            var email = principal.FindFirstValue(ClaimTypes.Email)
                ?? principal.FindFirstValue(JwtRegisteredClaimNames.Email);

            return (await sender.Send(new CompleteProfileCommand(userId, email), ct)).ToHttpResult();
        }).RequireAuthorization();
        return group;
    }
}
