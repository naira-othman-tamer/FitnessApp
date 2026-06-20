using System.Security.Cryptography;
using System.Text;
using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository.Layer.Interfaces;

namespace AuthenticationService.Features.Auth.VerifyOtp;

public sealed record VerifyOtpCommand(string Email, string Otp) : IRequest<OperationResult<VerifyOtpResponse>>;
public sealed record VerifyOtpResponse(string ResetToken);

public sealed class VerifyOtpHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork<Data.AuthenticationDbContext> unitOfWork,
    ITokenService tokens) : IRequestHandler<VerifyOtpCommand, OperationResult<VerifyOtpResponse>>
{
    public async Task<OperationResult<VerifyOtpResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var otp = await unitOfWork.Repository<OtpCode, Guid>().Query(asNoTracking: false)
            .Where(x => x.Email == email && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(cancellationToken);
        if (otp is null || otp.ExpiresAt <= DateTime.UtcNow || !FixedEquals(otp.Code, tokens.HashOpaqueToken(request.Otp)))
            return OperationResultFactory.BadRequest<VerifyOtpResponse>("Invalid or expired OTP.", "رمز التحقق غير صحيح أو منتهي");

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return OperationResultFactory.BadRequest<VerifyOtpResponse>("Invalid or expired OTP.", "رمز التحقق غير صحيح أو منتهي");

        otp.IsUsed = true;
        await unitOfWork.CompleteAsync();
        return OperationResultFactory.Success(new VerifyOtpResponse(tokens.CreateResetToken(user)));
    }

    private static bool FixedEquals(string left, string right) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(left), Encoding.UTF8.GetBytes(right));
}

public static class VerifyOtpEndpoint
{
    public sealed record Request(string Email, string Otp);
    public static RouteGroupBuilder MapVerifyOtp(this RouteGroupBuilder group)
    {
        group.MapPost("/verify-otp", async (Request request, ISender sender, CancellationToken ct) =>
            (await sender.Send(new VerifyOtpCommand(request.Email, request.Otp), ct)).ToHttpResult()).AllowAnonymous();
        return group;
    }
}
