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
            var statsGroup = builder.MapGroup("stats");
            statsGroup.SubmitFitnessStateEndPoint();
            statsGroup.GetUserStatsEndPoint();

            var metricsGroup = builder.MapGroup("metrics");
            metricsGroup.SubmitCalculateMetricsEndPoint();
            metricsGroup.GetUserMetricsEndpoint();
            metricsGroup.UpdateCalculateMetricsEndPoint();

            var planGroup = builder.MapGroup("plan");
            planGroup.AssignUserPlanEndPointEndPoint();
            planGroup.GetActiveUserPlanEndPoint();



            return builder;
        }
    }

}
