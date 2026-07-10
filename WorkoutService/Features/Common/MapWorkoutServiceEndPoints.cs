using WorkoutService.Features.Excercise.CreateExercies;
using WorkoutService.Features.Excercise.GetExerciseById;
using WorkoutService.Features.Excercise.GetExercisesList;
using WorkoutService.Features.WorkoutPlan.CreateWorkoutPlan;
using WorkoutService.Features.WorkoutPlan.FilterWorkoutPlans;
using WorkoutService.Features.WorkoutPlan.GetWorkoutPlanById;
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
            planGroup.MapGetPlanByIdEndPoint();
            planGroup.MapGetFilteredPlansEndPoint();
            planGroup.MapCreatePlanEndPoint();

            var workoutGroup = builder.MapGroup("workouts");
            workoutGroup.MapGetWorkoutByIdEndpoint();
            workoutGroup.MapCreateWorkoutEndPoint();
            workoutGroup.MapGetFilteredWorkoutsEndPoint();

            var workoutExerciseGroup = builder.MapGroup("workouts/exercises");
            workoutExerciseGroup.MapAddExerciseToWorkoutEndpoint();

            var ExerciseGroup = builder.MapGroup("Exercises");
            ExerciseGroup.MapGetExerciseByIdEndPoint();
            ExerciseGroup.MapGetFilteredExerciseListEndPoint();
            ExerciseGroup.MapCreateExerciseEndpoint();

            return builder;
        }
    }
}
