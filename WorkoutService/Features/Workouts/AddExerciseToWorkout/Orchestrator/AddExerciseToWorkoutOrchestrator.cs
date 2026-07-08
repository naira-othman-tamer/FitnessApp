using MediatR;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Features.Excercise.Shared;
using WorkoutService.Features.Workouts.AddExerciseToWorkout.Commands;
using WorkoutService.Features.Workouts.AddExerciseToWorkout.Queries;
using WorkoutService.Features.Workouts.Shared;

namespace WorkoutService.Features.Workouts.AddExerciseToWorkout.Orchestrator
{
    public record AddExerciseToWorkoutOrchestrator(
        int workoutId,
        int exerciseId,
        int ExSets,
        int ExReps,
        int ExRestInSeconds) : IRequest<RequestResult<bool>>;

    public class AddExerciseToWorkoutOrchestratorHandler : IRequestHandler<AddExerciseToWorkoutOrchestrator, RequestResult<bool>>
    {
        private readonly IMediator _mediator;
        public AddExerciseToWorkoutOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<RequestResult<bool>> Handle(AddExerciseToWorkoutOrchestrator request, CancellationToken cancellationToken)
        {
            var workoutExistResult = await _mediator.Send(new IsWorkoutExistQuery(request.workoutId), cancellationToken);
            if (!workoutExistResult.IsSuccess)
            {
                return RequestResult<bool>.Failure($"Workout with ID {request.workoutId} not found.");
            }

            var exerciseExistResult = await _mediator.Send(new IsExerciseExistQuery(request.exerciseId), cancellationToken);
            if (!exerciseExistResult.IsSuccess)
            {
                return RequestResult<bool>.Failure($"Exercise with ID {request.exerciseId} not found.");
            }

            var isExerciseAlreadyInWorkout = await _mediator
                .Send(new IsExerciseInWorkoutQuery(request.workoutId, request.exerciseId), cancellationToken);
            if (isExerciseAlreadyInWorkout.Data != false)
            {
                return RequestResult<bool>
                    .Failure($"Error checking if exercise with ID {request.exerciseId} is already in workout with ID {request.workoutId}.");
            }

            var workoutExerciseCountResult = await _mediator
                .Send(new GetWorkoutExerciseCountQuery(request.workoutId), cancellationToken);
            int orderIndex = workoutExerciseCountResult.Data + 1; 

            var AddExerciseToWorkoutResult = await _mediator.Send(new AddExerciseToWorkoutCommand(
                request.workoutId,
                request.exerciseId,
                orderIndex,
                request.ExSets,
                request.ExReps,
                request.ExRestInSeconds),
                cancellationToken);

            if (!AddExerciseToWorkoutResult.IsSuccess)
            {
                return RequestResult<bool>.Failure();
            }
            return RequestResult<bool>.Success(true);
        }
    }


}
