using ContractMessages.Enums;
using FluentValidation;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Features.Excercise.GetExercisesList;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.WorkoutPlan.FilterWorkoutPlans
{
    public record GetFilteredWorkoutPlansQuery(
        int PageIndex,
        int PageSize,
        string? Name,
        Goal? Goal,
        int? WorkoutDaysPerWeek
    ) : IRequest<RequestResult<PaginatedFilterPlansResultDto>>;

    public record PaginatedFilterPlansResultDto
    (
        int PageIndex,
        int PageSize,
        IEnumerable<GetPlansDto> filteredPlans
    );

    public record GetPlansDto
    (
     string Name,
     string? ImageUrl,
     bool IsPremium,
     Goal Goal,
     int WorkoutDaysPerWeek
    );

    public class GetFilteredWorkoutPlansQueryValidator : AbstractValidator<GetFilteredWorkoutPlansQuery>
    {
        public GetFilteredWorkoutPlansQueryValidator()
        {
            RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

            RuleFor(x => x.Name)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Goal!.Value)
                .IsInEnum()
                .When(x => x.Goal.HasValue);

            RuleFor(x => x.WorkoutDaysPerWeek!.Value)
                .InclusiveBetween(1, 7)
                .When(x => x.WorkoutDaysPerWeek.HasValue);
        }
    }

    public class GetFilteredWorkoutPlansQueryHandler
        : IRequestHandler<GetFilteredWorkoutPlansQuery, RequestResult<PaginatedFilterPlansResultDto>>
    {
        private readonly GeneralRepository<Domain.Entities.WorkoutPlan> _workoutPlanRepository;

        public GetFilteredWorkoutPlansQueryHandler(GeneralRepository<Domain.Entities.WorkoutPlan> workoutPlanRepository)
        {
            _workoutPlanRepository = workoutPlanRepository;
        }

        public async Task<RequestResult<PaginatedFilterPlansResultDto>> Handle(GetFilteredWorkoutPlansQuery request, CancellationToken cancellationToken)
        {
            var predicate = BuildPredicate(request);
            var Plans = _workoutPlanRepository.GetAll().AsExpandable();
            var paginatedResult = await Plans
                .Where(predicate)
                .Select(p => new GetPlansDto(
                    p.Name,
                    p.ImageUrl,
                    p.IsPremium,
                    p.Goal,
                    p.WorkoutDaysPerWeek))
                .ToPaginatedAsync(request.PageIndex, request.PageSize, cancellationToken);

            var result = new PaginatedFilterPlansResultDto(request.PageIndex, request.PageSize, paginatedResult.Data);
            if (paginatedResult.Data.Count == 0) {
                return RequestResult<PaginatedFilterPlansResultDto>.Failure("No workout plans found for the given filters.", RequestErrorCode.NotFound);
            }
            return RequestResult<PaginatedFilterPlansResultDto>.Success(result);
        }

        private static Expression<Func<Domain.Entities.WorkoutPlan, bool>> BuildPredicate(GetFilteredWorkoutPlansQuery query)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.WorkoutPlan>(true);

            if (!string.IsNullOrWhiteSpace(query.Name))
                predicate = predicate.And(p =>
                                     p.Name.ToLower().Contains(query.Name.ToLower()));

            if (query.Goal.HasValue)
                predicate = predicate.And(p => p.Goal == query.Goal.Value);

            if (query.WorkoutDaysPerWeek.HasValue)
                predicate = predicate.And(p => p.WorkoutDaysPerWeek == query.WorkoutDaysPerWeek.Value);

            return predicate;
        }
    }


    public static class GetFilteredWorkoutPlansQueryEndpoint
    {
        public static void MapGetFilteredPlansEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/", async ([FromServices] IMediator mediator,
                 int PageIndex = 1,
                 int PageSize = 10,
                 string? Name = null,
                 Goal? Goal = null,
                 int? WorkoutDaysPerWeek = null
               ) =>
            {
                var Plans = await mediator.Send(new GetFilteredWorkoutPlansQuery(
                    PageIndex,
                    PageSize,
                    Name,
                    Goal,
                    WorkoutDaysPerWeek));

                return;
            });
        }
    }
}
