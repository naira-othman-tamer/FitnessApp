using MediatR;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.ValueObject;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.AddExerciseToWorkout.Commands
{
    public record AddExerciseToWorkoutCommand (
        int workoutId,
        int exerciseId,
        int orderIndex,
        int ExSets,
        int ExReps,
        int ExRestInSeconds) : IRequest<RequestResult<bool>>;

    public class AddExerciseToWorkoutCommandHandler : IRequestHandler<AddExerciseToWorkoutCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<WorkoutExercise> _workoutExerciseRepository;
        public AddExerciseToWorkoutCommandHandler(GeneralRepository<WorkoutExercise> workoutRepository)
        {
            _workoutExerciseRepository = workoutRepository;
        }
        public async Task<RequestResult<bool>> Handle(AddExerciseToWorkoutCommand request, CancellationToken cancellationToken)
        {
            var ExercisePrescriptio = ExercisePrescription.Create(request.ExSets,request.ExReps,request.ExRestInSeconds);
            var workoutExercise = WorkoutExercise.Create
            (
                request.workoutId,
                request.exerciseId,
                request.orderIndex,
                ExercisePrescriptio
            );

            _workoutExerciseRepository.Add(workoutExercise);
            await _workoutExerciseRepository.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }

    }

}
