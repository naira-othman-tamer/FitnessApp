using FCE.Features.Metrics;
using FCE.Features.Stats.SubmitFitnessStats;

namespace FCE.Features.Common
{
    public static class MapFitnessCalculatorEngineEndPoints
    {
        public static IEndpointRouteBuilder MapFCEEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapSubmitFitnessStateEndPoint();
            builder.MapSubmitCalculateMetricsEndPoint();
            return builder;
            //var group = endpoints.MapGroup("/api/v1/auth").WithTags("Authentication");
            //group.MapRegister();
            //group.MapCompleteProfile();
            //group.MapLogin();
            //group.MapForgotPassword();
            //group.MapVerifyOtp();
            //group.MapResetPassword();
            //group.MapRefreshToken();
            //group.MapLogout();
            //return endpoints;
        }
    }

}
