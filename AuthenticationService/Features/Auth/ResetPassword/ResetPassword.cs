using AuthenticationService.Common.StandardizedResponse;
using ContractMessages.Notifications;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Infrastructure;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Repository.Layer.Interfaces;

namespace AuthenticationService.Features.Auth.ResetPassword;

public sealed record ResetPasswordCommand(string ResetToken, string NewPassword, string ConfirmPassword)
    : IRequest<OperationResult<ResetPasswordResponse>>;
public sealed record ResetPasswordResponse(bool PasswordChanged);

public sealed class ResetPasswordHandler(
    UserManager<ApplicationUser> userManager,
    ITokenService tokens,
    IUnitOfWork<Data.AuthenticationDbContext> unitOfWork,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<ResetPasswordCommand, OperationResult<ResetPasswordResponse>>
{
    public async Task<OperationResult<ResetPasswordResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.ConfirmPassword)
            return OperationResultFactory.BadRequest<ResetPasswordResponse>("Passwords do not match.", "كلمتا المرور غير متطابقتين");

        ResetTokenClaims claims;
        try { claims = tokens.ValidateResetToken(request.ResetToken); }
        catch (SecurityTokenException)
        {
            return OperationResultFactory.BadRequest<ResetPasswordResponse>("Invalid or expired reset token.", "رمز إعادة التعيين غير صالح أو منتهي");
        }

        var user = await userManager.FindByIdAsync(claims.UserId.ToString());
        if (user is null || !string.Equals(user.SecurityStamp, claims.SecurityStamp, StringComparison.Ordinal))
            return OperationResultFactory.BadRequest<ResetPasswordResponse>("Invalid or expired reset token.", "رمز إعادة التعيين غير صالح أو منتهي");

        var identityToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, identityToken, request.NewPassword);
        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(x => x.Description));
            return OperationResultFactory.BadRequest<ResetPasswordResponse>(message, message);
        }

        await unitOfWork.Repository<Domain.Entities.RefreshToken, Guid>()
            .UpdateWhereAsync(x => x.UserId == user.Id && x.RevokedAt == null,
                setters => setters.SetProperty(x => x.RevokedAt, DateTime.UtcNow));
        await unitOfWork.CompleteAsync();

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            await publishEndpoint.Publish<IEmailNotificationRequested>(new
            {
                NotificationId = Guid.NewGuid(),
                To = user.Email,
                Subject = "Your password was changed",
                Body = """
                       <p>Your Fitness App password was changed successfully.</p>
                       <p>If this was not you, reset your password immediately and contact support.</p>
                       """,
                IsHtml = true,
                RequestedAtUtc = DateTime.UtcNow
            }, cancellationToken);
        }

        return OperationResultFactory.Success(new ResetPasswordResponse(true));
    }
}

public static class ResetPasswordEndpoint
{
    public sealed record Request(string ResetToken, string NewPassword, string ConfirmPassword);
    public static RouteGroupBuilder MapResetPassword(this RouteGroupBuilder group)
    {
        group.MapPost("/reset-password", async (Request request, ISender sender, CancellationToken ct) =>
            (await sender.Send(new ResetPasswordCommand(request.ResetToken, request.NewPassword, request.ConfirmPassword), ct)).ToHttpResult())
            .AllowAnonymous();
        return group;
    }
}
