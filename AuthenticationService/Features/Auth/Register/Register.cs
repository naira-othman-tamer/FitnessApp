using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using AuthenticationService.Common.StandardizedResponse;
using AuthenticationService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ApiStatusCode = AuthenticationService.Common.StandardizedResponse.StatusCode;

namespace AuthenticationService.Features.Auth.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber) : IRequest<OperationResult<RegisterResponse>>;

public sealed record RegisterResponse(Guid UserId, bool RequiresProfileCompletion);

public sealed partial class RegisterHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<RegisterCommand, OperationResult<RegisterResponse>>
{
    public async Task<OperationResult<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var error = Validate(request);
        if (error is not null)
            return OperationResultFactory.BadRequest<RegisterResponse>(error, error);

        var email = request.Email.Trim().ToLowerInvariant();
        if (await userManager.FindByEmailAsync(email) is not null)
            return new OperationResult<RegisterResponse>(ApiStatusCode.Conflict, "Email is already registered.", "البريد الإلكتروني مسجل بالفعل");

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(x => x.Description));
            return OperationResultFactory.BadRequest<RegisterResponse>(message, message);
        }

        return OperationResultFactory.Success(
            new RegisterResponse(user.Id, true),
            statusCode: ApiStatusCode.Created);
    }

    private static string? Validate(RegisterCommand request)
    {
        if (request.FirstName.Trim().Length is < 2 or > 50 || request.LastName.Trim().Length is < 2 or > 50)
            return "First and last names must each contain between 2 and 50 characters.";
        if (!new EmailAddressAttribute().IsValid(request.Email))
            return "A valid email is required.";
        if (!EgyptianPhoneRegex().IsMatch(request.PhoneNumber))
            return "Phone number must be a valid Egyptian mobile number in international format.";
        return null;
    }

    [GeneratedRegex(@"^\+20(?:10|11|12|15)\d{8}$")]
    private static partial Regex EgyptianPhoneRegex();
}

public static class RegisterEndpoint
{
    public static RouteGroupBuilder MapRegister(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (RegisterCommand request, ISender sender, CancellationToken ct) =>
            (await sender.Send(request, ct)).ToHttpResult()).AllowAnonymous();
        return group;
    }
}
