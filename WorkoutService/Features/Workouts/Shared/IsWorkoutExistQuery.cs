using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.Shared
{
    public record IsWorkoutExistQuery(int workoutId) : IRequest<RequestResult<bool>>;

    public class IsWorkoutExistQueryHandler : IRequestHandler<IsWorkoutExistQuery, RequestResult<bool>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;
        public IsWorkoutExistQueryHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }
        public async Task<RequestResult<bool>> Handle(IsWorkoutExistQuery request, CancellationToken cancellationToken)
        {
            var exists = await _workoutRepository
                .Get(w => w.Id == request.workoutId)
                .AnyAsync(cancellationToken);

            if (!exists)
            {
                return RequestResult<bool>.Failure($"Workout with ID {request.workoutId} not found.");
            }

            return RequestResult<bool>.Success(true);
        }
    }

}
