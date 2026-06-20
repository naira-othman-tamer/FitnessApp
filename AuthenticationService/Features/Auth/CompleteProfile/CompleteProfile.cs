using System.Security.Claims;
using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Infrastructure;
using MediatR;

namespace AuthenticationService.Features.Auth.CompleteProfile;

public sealed record CompleteProfileCommand(Guid UserId) : IRequest<OperationResult>;

public sealed class CompleteProfileHandler(IProfileLifecyclePublisher publisher, IAccountStateCache cache)
    : IRequestHandler<CompleteProfileCommand, OperationResult>
{
    public async Task<OperationResult> Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
    {
        await publisher.PublishProfileLifecycleInitiatedAsync(request.UserId, cancellationToken);
        await cache.MarkProfileCompletedAsync(request.UserId, cancellationToken);
        return OperationResultFactory.Success(
            message: "Profile lifecycle initiated.",
            messageLocalized: "تم بدء دورة الملف الشخصي");
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
            return (await sender.Send(new CompleteProfileCommand(userId), ct)).ToHttpResult();
        }).RequireAuthorization();
        return group;
    }
}
