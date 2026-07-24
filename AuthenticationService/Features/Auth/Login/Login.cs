using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Data;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Infrastructure;
using ContractMessages.Notifications;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository.Layer.Interfaces;
using ApiStatusCode = AuthenticationService.Common.StandardizedResponse.StatusCode;

namespace AuthenticationService.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password, string IpAddress)
    : IRequest<OperationResult<LoginResponse>>;
public sealed record LoginResponse(string Token, string RefreshToken, bool ProfileCompleted, bool IsPremium);

public sealed class LoginHandler(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IUnitOfWork<AuthenticationDbContext> unitOfWork,
    ITokenService tokens,
    IAccountStateCache accountStateCache,
    IPublishEndpoint publishEndpoint,
    IOptions<JwtOptions> options) : IRequestHandler<LoginCommand, OperationResult<LoginResponse>>
{
    public async Task<OperationResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userManager.FindByEmailAsync(email);
        var wasAlreadyLocked = user?.IsLockedOut == true;

        if (user is not null && user.AccessFailedCount > 0)
        {
            var windowStart = DateTime.UtcNow.AddMinutes(-15);
            var hasRecentFailure = await unitOfWork.Repository<LoginAttempt, Guid>().Query()
                .AnyAsync(x => x.Email == email && !x.IsSuccess && x.AttemptedAt >= windowStart, cancellationToken);
            if (!hasRecentFailure)
                await userManager.ResetAccessFailedCountAsync(user);
        }

        var signIn = user is null
            ? SignInResult.Failed
            : await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        await unitOfWork.Repository<LoginAttempt, Guid>().Create(new LoginAttempt
        {
            Id = Guid.NewGuid(),
            Email = email,
            AttemptedAt = DateTime.UtcNow,
            IsSuccess = signIn.Succeeded,
            IpAddress = request.IpAddress
        });

        if (signIn.IsLockedOut && user is not null)
        {
            user.IsLockedOut = true;
            user.LockedUntil = user.LockoutEnd?.UtcDateTime;
            await userManager.UpdateAsync(user);
            await unitOfWork.CompleteAsync();

            if (!wasAlreadyLocked && !string.IsNullOrWhiteSpace(user.Email))
            {
                await publishEndpoint.Publish<IEmailNotificationRequested>(new
                {
                    NotificationId = Guid.NewGuid(),
                    To = user.Email,
                    Subject = "Your account was temporarily locked",
                    Body = $"""
                            <p>Your Fitness App account was temporarily locked after multiple failed login attempts.</p>
                            <p><strong>IP address:</strong> {request.IpAddress}</p>
                            <p>If this was not you, reset your password immediately.</p>
                            """,
                    IsHtml = true,
                    RequestedAtUtc = DateTime.UtcNow
                }, cancellationToken);
            }

            return new OperationResult<LoginResponse>(ApiStatusCode.Locked, "AUTH_ACCOUNT_LOCKED", "Account is temporarily locked.");
        }

        if (!signIn.Succeeded || user is null)
        {
            await unitOfWork.CompleteAsync();
            return OperationResultFactory.UnAuthorized<LoginResponse>("Invalid email or password.", "Invalid email or password.");
        }

        user.IsLockedOut = false;
        user.LockedUntil = null;
        var access = tokens.CreateAccessToken(user, await userManager.GetRolesAsync(user));
        var rawRefreshToken = tokens.CreateRefreshToken();
        await unitOfWork.Repository<Domain.Entities.RefreshToken, Guid>().Create(new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokens.HashOpaqueToken(rawRefreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(options.Value.RefreshTokenDays)
        });
        await userManager.UpdateAsync(user);
        await unitOfWork.CompleteAsync();
        var state = await accountStateCache.GetAsync(user.Id, cancellationToken);
        return OperationResultFactory.Success(new LoginResponse(access.Token, rawRefreshToken, state.ProfileCompleted, state.IsPremium));
    }
}

public static class LoginEndpoint
{
    public sealed record LoginRequest(string Email, string Password);

    public static RouteGroupBuilder MapLogin(this RouteGroupBuilder group)
    {
        group.MapPost("/login", async (LoginRequest request, HttpContext context, ISender sender, CancellationToken ct) =>
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return (await sender.Send(new LoginCommand(request.Email, request.Password, ip), ct)).ToHttpResult();
        }).AllowAnonymous();
        return group;
    }
}
