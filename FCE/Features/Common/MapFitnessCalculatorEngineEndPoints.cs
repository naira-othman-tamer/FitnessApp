using FCE.Features.Metrics;
using FCE.Features.Plan;
using FCE.Features.Stats.GetUsetStats;
using FCE.Features.Stats.SubmitFitnessStats;

namespace FCE.Features.Common
{
    public static class MapFitnessCalculatorEngineEndPoints
    {
        public static IEndpointRouteBuilder MapFCEEndpoints(this IEndpointRouteBuilder builder)
        {
            var statsGroup = builder.MapGroup("stats");
            statsGroup.SubmitFitnessStateEndPoint();
            statsGroup.GetUserStatsEndPoint();

            var metricsGroup = builder.MapGroup("metrics");
            metricsGroup.SubmitCalculateMetricsEndPoint();
            metricsGroup.GetUserMetricsEndpoint();

            var planGroup = builder.MapGroup("plan");
            planGroup.GetMatchedUserActivePlanEndpoint();

            return builder;
        }
    }

}
