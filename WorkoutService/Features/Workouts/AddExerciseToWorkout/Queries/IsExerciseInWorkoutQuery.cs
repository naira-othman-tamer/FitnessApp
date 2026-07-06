using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.AddExerciseToWorkout.Queries
{
    public record IsExerciseInWorkoutQuery(int workoutId, int exerciseId) : IRequest<RequestResult<bool>>;

    public class IsExerciseInWorkoutQueryHandler : IRequestHandler<IsExerciseInWorkoutQuery, RequestResult<bool>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;
        public IsExerciseInWorkoutQueryHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }
        public async Task<RequestResult<bool>> Handle(IsExerciseInWorkoutQuery request, CancellationToken cancellationToken)
        {
            var alreadyAdded = await _workoutRepository
                .Get(w => w.Id == request.workoutId)
                .SelectMany(w => w.WorkoutExercises.Select(we => we.ExerciseId))
                .AnyAsync(id => id == request.exerciseId, cancellationToken);

            if (alreadyAdded)
            {
                return RequestResult<bool>.Failure($"Exercise with ID {request.exerciseId} is already in this workout.");
            }
            return RequestResult<bool>.Success(false);
        }
    }

}
