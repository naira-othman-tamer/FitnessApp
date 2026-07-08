using WorkoutService.Features.WorkoutPlan.CreateWorkoutPlan;
using WorkoutService.Features.WorkoutPlan.FilterWorkoutPlans;

namespace WorkoutService.Features.Common
{
    public static class MapWorkoutServiceEndPoints
    {
        public static IEndpointRouteBuilder MapWorkoutEndpoints(this IEndpointRouteBuilder builder)
        {
            var statsGroup = builder.MapGroup("stats");

            var metricsGroup = builder.MapGroup("metrics");

            var planGroup = builder.MapGroup("plan");
            planGroup.GetFilteredPlansEndPoint();
            planGroup.AddPlanEndPoint();

            return builder;
        }
    }
}
