using FluentValidation;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.Workouts.GetWorkoutsList
{
    public record GetFilteredWorkoutsQuery(
        int Page,
        int PageSize,
        WorkoutCategory? Category,
        bool? IsPremium) : IRequest<RequestResult<PaginatedFilteredWorkoutsListDto>>;

    public class GetFilteredWorkoutsQueryValidator : AbstractValidator<GetFilteredWorkoutsQuery>
    {
        public GetFilteredWorkoutsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.");
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid workout category.");
        }
    }
    public record PaginatedFilteredWorkoutsListDto
     (
        int page,
        int pageSize,
        IEnumerable<GetFilteredWorkoutsDto> filteredWorkouts
     );

    public record GetFilteredWorkoutsDto
    (
        string Name,
        string? ImageUrl,
        bool IsPremium,
        WorkoutCategory Category,
        int DurationInMinutes
    );

    public class GetFilteredWorkoutsQueryHandler
        : IRequestHandler<GetFilteredWorkoutsQuery, RequestResult<PaginatedFilteredWorkoutsListDto>>
    {
        private readonly GeneralRepository<Domain.Entities.Workout> _workoutRepository;
        public GetFilteredWorkoutsQueryHandler(GeneralRepository<Domain.Entities.Workout> workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }
        public async Task<RequestResult<PaginatedFilteredWorkoutsListDto>> Handle(GetFilteredWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var predicate = BuildPredicate(request);
            var workouts = _workoutRepository.GetAll();
            var paginatedResult = await workouts.Where(predicate)
                .Select(w => new GetFilteredWorkoutsDto(
                    w.Name,
                    w.ImageUrl,
                    w.IsPremium,
                    w.Category,
                    w.DurationInMinutes
                )).ToPaginatedAsync(request.Page, request.PageSize, cancellationToken);

            var result = new PaginatedFilteredWorkoutsListDto(request.Page, request.PageSize, paginatedResult.Data);
            if (paginatedResult.Data.Count() == 0)
            {
                return RequestResult<PaginatedFilteredWorkoutsListDto>.Failure("No workouts found for the given filters.", RequestErrorCode.NotFound);
            }
            return RequestResult<PaginatedFilteredWorkoutsListDto>.Success(result);
        }

        private static Expression<Func<Domain.Entities.Workout, bool>> BuildPredicate(GetFilteredWorkoutsQuery request)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.Workout>(true);
            if (request.Category.HasValue)
            {
                predicate = predicate.And(w => w.Category == request.Category.Value);
            }
            if (request.IsPremium.HasValue)
            {
                predicate = predicate.And(w => w.IsPremium == request.IsPremium.Value);
            }
            return predicate;
        }
    }

    public static class GetFilteredWorkoutPlansEndPont
    {
        public static void GetFilteredWorkoutsEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", async ([FromServices] IMediator mediator,
                 int PageIndex = 1,
                 int PageSize = 10,
                 WorkoutCategory? Category = null,
                 bool? IsPremium = null
               ) =>
            {
                var workouts = await mediator.Send(new GetFilteredWorkoutsQuery(
                    PageIndex,
                    PageSize,
                    Category,
                   IsPremium));
                return Results.Ok(workouts.Data);
            });
        }
    }
}
