using MediatR;
using ApiStatusCode = ProfileService.Common.StandardizedResponse.StatusCode;

namespace ProfileService.Features.Profile.ChangePassword;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword) : IRequest<OperationResult>;

public sealed class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, OperationResult>
{
    public Task<OperationResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        OperationResult result = new(
            ApiStatusCode.NotImplemented,
            "Password changes are owned by AuthenticationService. Wire this endpoint as an AuthService proxy if the API contract must stay here.",
            "Password changes are owned by AuthenticationService.");

        return Task.FromResult(result);
    }
}
