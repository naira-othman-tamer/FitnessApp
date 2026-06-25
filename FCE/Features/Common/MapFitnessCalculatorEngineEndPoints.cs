using FCE.Features.Metrics;
using FCE.Features.Plan;
using FCE.Features.Stats.SubmitFitnessStats;

namespace FCE.Features.Common
{
    public static class MapFitnessCalculatorEngineEndPoints
    {
        public static IEndpointRouteBuilder MapFCEEndpoints(this IEndpointRouteBuilder builder)
        {
            var statsGroup = builder.MapGroup("stats");
            statsGroup.MapSubmitFitnessStateEndPoint();

            var metricsGroup = builder.MapGroup("metrics");
            metricsGroup.MapSubmitCalculateMetricsEndPoint();

            var planGroup = builder.MapGroup("plan");
            planGroup.GetMatchedUserActivePlanEndpoint();

            return builder;
            //var group = endpoints.MapGroup("/api/v1/auth").WithTags("Authentication");
            //group.MapRegister();
            //group.MapCompleteProfile();

            //return endpoints;
        }
    }

}
