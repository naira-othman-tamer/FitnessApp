using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.AddExerciseToWorkout.Queries
{
    public record GetWorkoutExerciseCountQuery(int workoutId) : IRequest<RequestResult<int>>;

    public class GetWorkoutExerciseCountQueryHandler : IRequestHandler<GetWorkoutExerciseCountQuery, RequestResult<int>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;
        public GetWorkoutExerciseCountQueryHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }
        public async Task<RequestResult<int>> Handle(GetWorkoutExerciseCountQuery request, CancellationToken cancellationToken)
        {
            var exists = await _workoutRepository.Get(w => w.Id == request.workoutId).AnyAsync(cancellationToken);
            if (!exists)
            {
                return RequestResult<int>.Failure($"Workout with ID {request.workoutId} not found.");
            }
            var workout = await _workoutRepository
                .Get(w => w.Id == request.workoutId)
                .Select(w => w.WorkoutExercises.Count)
                .FirstOrDefaultAsync(cancellationToken);
            
            return RequestResult<int>.Success(workout);
        }
    }
}
