using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Infrastructure;
using MediatR;
using Repository.Layer.Interfaces;

namespace AuthenticationService.Features.Auth.Logout;

public sealed record LogoutCommand(Guid UserId, string Jti, DateTimeOffset TokenExpiresAt)
    : IRequest<OperationResult>;

public sealed class LogoutHandler(
    IUnitOfWork<Data.AuthenticationDbContext> unitOfWork,
    IAccessTokenRevocationStore revocations) : IRequestHandler<LogoutCommand, OperationResult>
{
    public async Task<OperationResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await revocations.RevokeAsync(request.Jti, request.TokenExpiresAt, cancellationToken);
        await unitOfWork.Repository<Domain.Entities.RefreshToken, Guid>()
            .UpdateWhereAsync(x => x.UserId == request.UserId && x.RevokedAt == null,
                setters => setters.SetProperty(x => x.RevokedAt, DateTime.UtcNow));
        await unitOfWork.CompleteAsync();
        return OperationResultFactory.Success(
            message: "Logged out successfully.",
            messageLocalized: "تم تسجيل الخروج بنجاح");
    }
}

public static class LogoutEndpoint
{
    public static RouteGroupBuilder MapLogout(this RouteGroupBuilder group)
    {
        group.MapPost("/logout", async (ClaimsPrincipal principal, ISender sender, CancellationToken ct) =>
        {
            if (!Guid.TryParse(principal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
                string.IsNullOrWhiteSpace(principal.FindFirstValue(JwtRegisteredClaimNames.Jti)) ||
                !long.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Exp), out var exp))
                return OperationResultFactory.UnAuthorized().ToHttpResult();

            var command = new LogoutCommand(userId, principal.FindFirstValue(JwtRegisteredClaimNames.Jti)!,
                DateTimeOffset.FromUnixTimeSeconds(exp));
            return (await sender.Send(command, ct)).ToHttpResult();
        }).RequireAuthorization();
        return group;
    }
}
