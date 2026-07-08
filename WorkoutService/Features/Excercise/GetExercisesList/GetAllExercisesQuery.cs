using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.GetExercisesList
{
    public record GetAllExercisesQuery(int pageNumber, int pageSize) : IRequest<RequestResult<GetAllExercisesDto>?>;

    public record GetAllExercisesDto
    (
        int pageNumber,
        int pageSize,
        int totalCount,
        IEnumerable<ExerciseDto> Exercises
    );

    public record ExerciseDto
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

    public class GetAllExercisesQueryHandler : IRequestHandler<GetAllExercisesQuery, RequestResult<GetAllExercisesDto?>>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public GetAllExercisesQueryHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResult<GetAllExercisesDto?>> Handle(GetAllExercisesQuery request, CancellationToken cancellationToken)
        {
            var (exercises, totalCount, totalPages) = await _exerciseRepository
                .GetAll()
                .ToPaginatedAsync(request.pageNumber, request.pageSize, cancellationToken);

            var exercisesDto = exercises
            .Select(e => new ExerciseDto(
                Id: e.Id,
                Name: e.Name,
                Description: e.Description,
                ImageUrl: e.ImageUrl,
                VideoUrl: e.VideoUrl,
                Difficulty: e.Difficulty.ToString(),
                TargetMuscles: e.TargetMuscles.Select(tm => tm.ToString()),
                EquipmentNeeded: e.EquipmentNeeded.Select(eq => eq.ToString())
            ));

            var result = new GetAllExercisesDto(
                pageNumber: request.pageNumber,
                pageSize: request.pageSize,
                totalCount: totalCount,
                Exercises: exercisesDto
            );

            return RequestResult<GetAllExercisesDto?>.Success(result);
        }
    }

}
