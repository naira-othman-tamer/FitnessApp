using MediatR;
using WorkoutService.Domain.Enums;
using WorkoutService.Domain.Entities;
using WorkoutService.Infrastructure;
using WorkoutService.Features.Common.Helpers;

namespace WorkoutService.Features.Workouts.CreateWorkout
{
    public record CreateWorkoutCommand
    (
     string Name ,
     WorkoutCategory Category ,
     Difficulty? Difficulty,
     int DurationInMinutes,
     string? ImageUrl ,
     bool IsPremium = false 
    ) : IRequest<RequestResult<bool>>;

    public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;

        public CreateWorkoutCommandHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public async Task<RequestResult<bool>> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            _workoutRepository.Add(Domain.Entities.Workout.Create(
                request.Name,
                request.Difficulty ?? Difficulty.Beginner,
                request.DurationInMinutes,
                caloriesBurn: 350, 
                request.Category,
                request.ImageUrl,
                request.IsPremium
            ));
            await _workoutRepository.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }
}
