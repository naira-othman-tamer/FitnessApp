using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.GetWorkoutById
{
    public record GetWorkoutByIdQuery(int WorkoutId) : IRequest<RequestResult<WorkoutDto>>;

    public record WorkoutDto
    (
        string Name,
        WorkoutCategory Category,
        Difficulty Difficulty,
        int DurationInMinutes,
        int CaloriesBurn,
        string? ImageUrl,
        bool IsPremium
    );

    public class GetWorkoutByIdQueryHandler : IRequestHandler<GetWorkoutByIdQuery, RequestResult<WorkoutDto>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;
        public GetWorkoutByIdQueryHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }
        public async Task<RequestResult<WorkoutDto>> Handle(GetWorkoutByIdQuery request, CancellationToken cancellationToken)
        {
            var workoutDto = await _workoutRepository.Get(w=>w.Id == request.WorkoutId)
                .Select(w => new WorkoutDto(
                    w.Name,
                    w.Category,
                    w.Difficulty,
                    w.DurationInMinutes,
                    w.CaloriesBurn,
                    w.ImageUrl,
                    w.IsPremium
                )).FirstOrDefaultAsync(cancellationToken);

            return RequestResult<WorkoutDto>.Success(workoutDto);
        }
    }

}
