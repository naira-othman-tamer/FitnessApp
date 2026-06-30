using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Repository.Layer.Interfaces;

namespace AuthenticationService.Features.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<OperationResult<RefreshTokenResponse>>;
public sealed record RefreshTokenResponse(string Token, string RefreshToken, bool ProfileCompleted, bool IsPremium);

public sealed class RefreshTokenHandler(
    IUnitOfWork<Data.AuthenticationDbContext> unitOfWork,
    ITokenService tokens,
    UserManager<ApplicationUser> userManager,
    IAccountStateCache accountStateCache,
    IOptions<JwtOptions> options) : IRequestHandler<RefreshTokenCommand, OperationResult<RefreshTokenResponse>>
{
    public async Task<OperationResult<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = tokens.HashOpaqueToken(request.RefreshToken);
        var repository = unitOfWork.Repository<Domain.Entities.RefreshToken, Guid>();
        var stored = await repository.Query(asNoTracking: false, x => x.User)
            .SingleOrDefaultAsync(x => x.Token == tokenHash, cancellationToken);
        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= DateTime.UtcNow)
            return OperationResultFactory.UnAuthorized<RefreshTokenResponse>("Invalid or expired refresh token.", "رمز التحديث غير صالح أو منتهي");

        stored.RevokedAt = DateTime.UtcNow;
        var rawRefreshToken = tokens.CreateRefreshToken();
        await repository.Create(new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(), UserId = stored.UserId, Token = tokens.HashOpaqueToken(rawRefreshToken),
            CreatedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddDays(options.Value.RefreshTokenDays)
        });
        await unitOfWork.CompleteAsync();

        var access = tokens.CreateAccessToken(stored.User, await userManager.GetRolesAsync(stored.User));
        var state = await accountStateCache.GetAsync(stored.UserId, cancellationToken);
        return OperationResultFactory.Success(
            new RefreshTokenResponse(access.Token, rawRefreshToken, state.ProfileCompleted, state.IsPremium));
    }
}

public static class RefreshTokenEndpoint
{
    public sealed record Request(string RefreshToken);
    public static RouteGroupBuilder MapRefreshToken(this RouteGroupBuilder group)
    {
        group.MapPost("/refresh-token", async (Request request, ISender sender, CancellationToken ct) =>
            (await sender.Send(new RefreshTokenCommand(request.RefreshToken), ct)).ToHttpResult()).AllowAnonymous();
        return group;
    }
}
