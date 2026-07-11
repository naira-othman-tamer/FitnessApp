using FCE.Features.Metrics.GetUserCurrentMetrics;
using FCE.Features.Metrics.RecalculateBioMetrics;
using FCE.Features.Metrics.SetUserCalculatedMetrics.Orchestrator;
using FCE.Features.Plan.AssignUserPlan;
using FCE.Features.Plan.GetActiveAssignedUserPlan;
using FCE.Features.Stats.Shared.GetUsetStats;
using FCE.Features.Stats.SubmitFitnessStats;

namespace FCE.Features.Common
{
    public static class MapFitnessCalculatorEngineEndPoints
    {
        public static IEndpointRouteBuilder MapFCEEndpoints(this IEndpointRouteBuilder builder)
        {
            var statsGroup = builder.MapGroup("stats").RequireAuthorization();
            statsGroup.MapSubmitFitnessStateEndPoint();
            statsGroup.MapGetUserStatsEndPoint();

            var metricsGroup = builder.MapGroup("metrics").RequireAuthorization();
            metricsGroup.MapSubmitCalculateMetricsEndPoint();
            metricsGroup.MapGetUserMetricsEndpoint();
            metricsGroup.MapReCalculateMetricsEndPoint();

            var planGroup = builder.MapGroup("plan").RequireAuthorization();
            planGroup.MapAssignUserPlanEndPointEndPoint();
            planGroup.MapGetActiveUserPlanEndPoint();
            return builder;
        }
    }

}
