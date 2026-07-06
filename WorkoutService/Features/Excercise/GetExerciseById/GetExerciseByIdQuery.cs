using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.GetExerciseById
{
    public record GetExerciseByIdQuery(int Id) : IRequest<RequestResult<GetExerciseDto>>;

    public record GetExerciseDto
    (
        int Id,
        string Name,
        string? Description,
        string? ImageUrl,
        string? VideoUrl,
        string Difficulty,
        IEnumerable<string> TargetMuscles,
        IEnumerable<string> EquipmentNeeded
    );

    public class GetExerciseByIdQueryHandler : IRequestHandler<GetExerciseByIdQuery, RequestResult<GetExerciseDto>>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public GetExerciseByIdQueryHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResult<GetExerciseDto>> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
        {
            var exercise = await _exerciseRepository.Get(e => e.Id == request.Id)
                .Select(e => new GetExerciseDto(
                    e.Id,
                    e.Name,
                    e.Description,
                    e.ImageUrl,
                    e.VideoUrl,
                    e.Difficulty.ToString(),
                    e.TargetMuscles.Select(m => m.ToString()),
                    e.EquipmentNeeded.Select(eq => eq.ToString())
                )
            ).FirstOrDefaultAsync(cancellationToken);

            if (exercise is null)
            {
                throw new KeyNotFoundException($"Exercise with ID {request.Id} not found.");
            }

            return RequestResult<GetExerciseDto>.Success(exercise);
        }
    }
}
