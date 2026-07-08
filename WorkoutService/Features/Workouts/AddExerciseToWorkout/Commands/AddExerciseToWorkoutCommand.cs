using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.ValueObject;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Features.WorkoutPlan.CreateWorkoutPlan;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.AddExerciseToWorkout.Commands
{
    public record AddExerciseToWorkoutCommand(
        int workoutId,
        int exerciseId,
        int orderIndex,
        int ExSets,
        int ExReps,
        int ExRestInSeconds) : IRequest<RequestResult<bool>>;

    public class AddExerciseToWorkoutCommandValidator : AbstractValidator<AddExerciseToWorkoutCommand>
    {
        public AddExerciseToWorkoutCommandValidator()
        {
            RuleFor(x => x.workoutId)
                .GreaterThan(0).WithMessage("Workout ID must be a positive integer.");
            RuleFor(x => x.exerciseId)
                .GreaterThan(0).WithMessage("Exercise ID must be a positive integer.");
            RuleFor(x => x.orderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Order index must be a non-negative integer.");
            RuleFor(x => x.ExSets).
                GreaterThan(0).WithMessage("Exercise sets must be a positive integer.");
            RuleFor(x => x.ExReps)
                .GreaterThan(0).WithMessage("Exercise reps must be a positive integer.");
            RuleFor(x => x.ExRestInSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("Exercise rest time must be a non-negative integer.");
        }
    }
    public class AddExerciseToWorkoutCommandHandler : IRequestHandler<AddExerciseToWorkoutCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<WorkoutExercise> _workoutExerciseRepository;
        public AddExerciseToWorkoutCommandHandler(GeneralRepository<WorkoutExercise> workoutRepository)
        {
            _workoutExerciseRepository = workoutRepository;
        }
        public async Task<RequestResult<bool>> Handle(AddExerciseToWorkoutCommand request, CancellationToken cancellationToken)
        {
            var ExercisePrescrip = ExercisePrescription.Create(request.ExSets, request.ExReps, request.ExRestInSeconds);
            var workoutExercise = WorkoutExercise.Create
            (
                request.workoutId,
                request.exerciseId,
                request.orderIndex,
                ExercisePrescrip
            );

            _workoutExerciseRepository.Add(workoutExercise);
            await _workoutExerciseRepository.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }
}
