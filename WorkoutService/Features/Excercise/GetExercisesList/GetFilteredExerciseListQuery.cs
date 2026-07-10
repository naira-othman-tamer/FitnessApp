using FluentValidation;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;
using static WorkoutService.Features.Common.Helpers.PaginationHelper;

namespace WorkoutService.Features.Excercise.GetExercisesList
{
    public record GetFilteredExerciseListQuery(
        int pageNumber,
        int pageSize,
        string? nameFilter,
        Difficulty? difficultyFilter) : IRequest<RequestResult<GetFilteredExerciseListDto>?>;

    public record GetFilteredExerciseListDto
    (
        int pageNumber,
        int pageSize,
        int totalCount,
        IEnumerable<ExerciseDto> FilteredExercises
    );

    public record ExerciseDto
    (
    int Id,
    string Name,
    string? Description,
    string? ImageUrl,
    string? VideoUrl,
    string Difficulty,
    IEnumerable<MuscleGroup> TargetMuscles,
    IEnumerable<EquipmentNeeded> EquipmentNeeded
    );

    public class GetFilteredExerciseListQueryValidator : AbstractValidator<GetFilteredExerciseListQuery>
    {
        public GetFilteredExerciseListQueryValidator()
        {
            RuleFor(x => x.pageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.pageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.");
            RuleFor(x => x.difficultyFilter)
                .IsInEnum().When(x => x.difficultyFilter.HasValue);
        }
    }

    public class GetFilteredExerciseListQueryHandler : IRequestHandler<GetFilteredExerciseListQuery, RequestResult<GetFilteredExerciseListDto?>>
    {
        private readonly GeneralRepository<Exercise> _exerciseRepository;
        public GetFilteredExerciseListQueryHandler(GeneralRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResult<GetFilteredExerciseListDto?>> Handle(GetFilteredExerciseListQuery request, CancellationToken cancellationToken)
        {
            var predicate = BuildFilterExpression(request.nameFilter, request.difficultyFilter);
            var Exercises = _exerciseRepository.GetAll().AsExpandable();
            var PaginatedResult = await Exercises.Where(predicate)
                .Select(E => new ExerciseDto(
                    E.Id,
                    E.Name,
                    E.Description,
                    E.ImageUrl,
                    E.VideoUrl,
                    E.Difficulty.ToString(),
                    E.TargetMuscles,
                    E.EquipmentNeeded
                ))
                .ToPaginatedAsync(request.pageNumber, request.pageSize, cancellationToken);

            var result = new GetFilteredExerciseListDto(
                request.pageNumber,
                request.pageSize,
                PaginatedResult.TotalCount,
                PaginatedResult.Data
            );

            if (result.FilteredExercises is null || !result.FilteredExercises.Any())
            {
                return RequestResult<GetFilteredExerciseListDto?>
                    .Failure("No exercises found matching the provided filters.", RequestErrorCode.NotFound);
            }

            return RequestResult<GetFilteredExerciseListDto?>.Success(result);
        }

        private static Expression<Func<Exercise, bool>> BuildFilterExpression(string? nameFilter, Difficulty? difficultyFilter)
        {
            var predicate = PredicateBuilder.New<Exercise>(true);
            if(!string.IsNullOrWhiteSpace(nameFilter))
            {
                predicate = predicate.And(e => e.Name.ToLower().Contains(nameFilter.ToLower()));
            }
            if(difficultyFilter.HasValue) {
                predicate = predicate.And(e => e.Difficulty == difficultyFilter.Value);
            }
            return predicate;
        }
    }

    public static class GetFilteredExerciseListQueryEndPoint
    {
        public static void MapGetFilteredExerciseListEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", async (
                [FromServices] IMediator mediator,
                int PageIndex = 1,
                int PageSize = 10,
                string? Name = null,
                Difficulty? Difficulty = null) =>
            {
                var result = await mediator.Send(new GetFilteredExerciseListQuery(
                    PageIndex,
                    PageSize,
                    Name,
                    Difficulty));

                if (!result.IsSuccess)
                {
                    return result.requestErrorCode switch
                    {
                        RequestErrorCode.NotFound => Results.NotFound(result.Message),
                        RequestErrorCode.ValidationError => Results.BadRequest(result.Message),
                        _ => Results.Problem(result.Message)
                    };
                }

                return Results.Ok(result.Data);
            });
        }
    }
}
