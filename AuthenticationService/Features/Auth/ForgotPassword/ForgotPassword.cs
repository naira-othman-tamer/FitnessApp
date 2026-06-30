using System.Security.Cryptography;
using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Layer.Interfaces;
using ApiStatusCode = AuthenticationService.Common.StandardizedResponse.StatusCode;

namespace AuthenticationService.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<OperationResult<ForgotPasswordResponse>>;
public sealed record ForgotPasswordResponse(string Email, int OtpExpiresIn, int CanResendIn);

public sealed class ForgotPasswordHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork<Data.AuthenticationDbContext> unitOfWork,
    ITokenService tokens,
    IOtpNotificationService notificationService)
    : IRequestHandler<ForgotPasswordCommand, OperationResult<ForgotPasswordResponse>>
{
    public async Task<OperationResult<ForgotPasswordResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var response = new ForgotPasswordResponse(email, 600, 30);
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return OperationResultFactory.Success(response);

        var repository = unitOfWork.Repository<OtpCode, Guid>();
        var lastOtp = await repository.Query().Where(x => x.Email == email)
            .OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(cancellationToken);
        var now = DateTime.UtcNow;
        if (lastOtp is not null && now - lastOtp.CreatedAt < TimeSpan.FromSeconds(30))
        {
            var remaining = 30 - (int)(now - lastOtp.CreatedAt).TotalSeconds;
            return new OperationResult<ForgotPasswordResponse>(ApiStatusCode.TooManyRequests,
                "OTP_RESEND_LIMIT", "يرجى الانتظار قبل إعادة إرسال الرمز", response with { CanResendIn = remaining });
        }

        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        await repository.Create(new OtpCode
        {
            Id = Guid.NewGuid(), Email = email, Code = tokens.HashOpaqueToken(otp),
            CreatedAt = now, ExpiresAt = now.AddMinutes(10), IsUsed = false
        });
        await unitOfWork.CompleteAsync();
        await notificationService.SendPasswordResetOtpAsync(email, otp, cancellationToken);
        return OperationResultFactory.Success(response);
    }
}

public static class ForgotPasswordEndpoint
{
    public sealed record Request(string Email);
    public static RouteGroupBuilder MapForgotPassword(this RouteGroupBuilder group)
    {
        group.MapPost("/forgot-password", async (Request request, ISender sender, CancellationToken ct) =>
            (await sender.Send(new ForgotPasswordCommand(request.Email), ct)).ToHttpResult()).AllowAnonymous();
        return group;
    }
}
