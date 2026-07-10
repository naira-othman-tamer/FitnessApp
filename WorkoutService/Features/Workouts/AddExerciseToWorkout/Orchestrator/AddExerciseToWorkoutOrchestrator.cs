using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

    public class AddExerciseToWorkoutOrchestratorValidator : AbstractValidator<AddExerciseToWorkoutOrchestrator>
    {
        public AddExerciseToWorkoutOrchestratorValidator()
        {
            RuleFor(x => x.workoutId)
                .GreaterThan(0).WithMessage("Workout ID must be greater than 0.");
            RuleFor(x => x.exerciseId)
                .GreaterThan(0).WithMessage("Exercise ID must be greater than 0.");
            RuleFor(x => x.ExSets)
                .GreaterThan(0).WithMessage("Number of sets must be greater than 0.");
            RuleFor(x => x.ExReps)
                .GreaterThan(0).WithMessage("Number of reps must be greater than 0.");
            RuleFor(x => x.ExRestInSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("Rest time in seconds must be non-negative.");
        }
    }
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
                return RequestResult<bool>.Failure($"Workout with ID {request.workoutId} not found.", workoutExistResult.requestErrorCode);
            }

            var exerciseExistResult = await _mediator.Send(new IsExerciseExistQuery(request.exerciseId), cancellationToken);
            if (!exerciseExistResult.IsSuccess)
            {
                return RequestResult<bool>.Failure($"Exercise with ID {request.exerciseId} not found.", exerciseExistResult.requestErrorCode);
            }

            var isExerciseAlreadyInWorkout = await _mediator
                .Send(new IsExerciseInWorkoutQuery(request.workoutId, request.exerciseId), cancellationToken);
            if (isExerciseAlreadyInWorkout.Data != false)
            {
                return RequestResult<bool>
                    .Failure($"Error checking if exercise with ID {request.exerciseId} is already in workout with ID {request.workoutId}.", isExerciseAlreadyInWorkout.requestErrorCode);
            }

            var workoutExerciseCountResult = await _mediator
                .Send(new GetWorkoutExerciseCountQuery(request.workoutId), cancellationToken);
            if (!workoutExerciseCountResult.IsSuccess)
            {
                return RequestResult<bool>
                    .Failure($"Error getting exercise count for workout with ID {request.workoutId}.", workoutExerciseCountResult.requestErrorCode);
            }
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
                return RequestResult<bool>
                    .Failure(AddExerciseToWorkoutResult.Message?? "Failed to add exercise to workout.", AddExerciseToWorkoutResult.requestErrorCode);
            }
            return RequestResult<bool>.Success(true);
        }
    }

    public static class AddExerciseToWorkoutEndPoint
    {
        public static void MapAddExerciseToWorkoutEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("AddExerciseToWorkout", async ([FromBody] AddExerciseToWorkoutOrchestrator request,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }
                return Results.Ok(result.Data);
            });
        }
    }


}
