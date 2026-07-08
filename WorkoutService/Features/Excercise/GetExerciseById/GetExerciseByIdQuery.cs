using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Domain.Entities;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Excercise.GetExerciseById
{
    public record GetExerciseByIdQuery(int Id) : IRequest<RequestResult<RequestResult<GetExerciseDto>>>;

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

    public class GetExerciseByIdQueryValidator : AbstractValidator<GetExerciseByIdQuery>
    {
        public GetExerciseByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Exercise ID must be greater than 0.");
        }
    }
    public class GetExerciseByIdQueryHandler : IRequestHandler<GetExerciseByIdQuery, RequestResult<RequestResult<GetExerciseDto>>>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public GetExerciseByIdQueryHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResult<RequestResult<GetExerciseDto>>> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
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
                return RequestResult<RequestResult<GetExerciseDto>>.Failure("Exercise not found.", RequestErrorCode.NotFound);
            }

            return RequestResult<RequestResult<GetExerciseDto>>.Success(RequestResult<GetExerciseDto>.Success(exercise));
        }
    }

    public static class GetExerciseByIdEndpoint
    {
        public static void MapGetExerciseByIdEndpoint(this WebApplication app)
        {
            app.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetExerciseByIdQuery(id));
                return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result);
            })
            .WithName("GetExerciseById");
            //.WithTags("Exercises");
        }
    }
}
