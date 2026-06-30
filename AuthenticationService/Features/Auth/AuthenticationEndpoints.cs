using AuthenticationService.Features.Auth.CompleteProfile;
using AuthenticationService.Features.Auth.ForgotPassword;
using AuthenticationService.Features.Auth.Login;
using AuthenticationService.Features.Auth.Logout;
using AuthenticationService.Features.Auth.RefreshToken;
using AuthenticationService.Features.Auth.Register;
using AuthenticationService.Features.Auth.ResetPassword;
using AuthenticationService.Features.Auth.VerifyOtp;

namespace AuthenticationService.Features.Auth;

public static class AuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth").WithTags("Authentication");
        group.MapRegister();
        group.MapCompleteProfile();
        group.MapLogin();
        group.MapForgotPassword();
        group.MapVerifyOtp();
        group.MapResetPassword();
        group.MapRefreshToken();
        group.MapLogout();
        return endpoints;
    }
}
