using WorkoutService.Features.WorkoutPlan.CreateWorkoutPlan;
using WorkoutService.Features.WorkoutPlan.FilterWorkoutPlans;
using WorkoutService.Features.Workouts.AddExerciseToWorkout.Orchestrator;
using WorkoutService.Features.Workouts.CreateWorkout;
using WorkoutService.Features.Workouts.GetWorkoutById;
using WorkoutService.Features.Workouts.GetWorkoutsList;

namespace WorkoutService.Features.Common
{
    public static class MapWorkoutServiceEndPoints
    {
        public static IEndpointRouteBuilder MapWorkoutEndpoints(this IEndpointRouteBuilder builder)
        {
            var planGroup = builder.MapGroup("plan");
            planGroup.GetFilteredPlansEndPoint();
            planGroup.AddPlanEndPoint();

            var workoutGroup = builder.MapGroup("workouts");
            workoutGroup.GetWorkoutByIDEndpoint();
            workoutGroup.AddWorkoutEndPoint();
            workoutGroup.GetFilteredWorkoutsEndPoint();

            var workoutExerciseGroup = builder.MapGroup("workouts/exercises");
            workoutExerciseGroup.AddExerciseToWorkoutEndpoint();

            var ExerciseGroup = builder.MapGroup("Exercises");
            //ExerciseGroup.MapCreateExerciseEndpoint();
            return builder;
        }
    }
}
