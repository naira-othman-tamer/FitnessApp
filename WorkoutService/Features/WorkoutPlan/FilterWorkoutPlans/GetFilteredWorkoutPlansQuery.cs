using ContractMessages.Enums;
using LinqKit;
using MediatR;
using System.Linq.Expressions;
using WorkoutService.Domain.Enums;
using WorkoutService.Features.Common.Helpers;
using WorkoutService.Infrastructure;

namespace WorkoutService.Features.WorkoutPlan.FilterWorkoutPlans
{
    public record GetFilteredWorkoutPlansQuery(
        int PageIndex,
        int PageSize,
        string? Name,
        Goal? Goal,
        Difficulty? Difficulty,
        int? WorkoutDaysPerWeek
    ) : IRequest<RequestResult<FilterPlansResultDto>>;

    public record FilterPlansResultDto
    (
        int Page,
        int PageSize,
        IEnumerable<GetPlansDto> filteredPlans
    );

    public record GetPlansDto
    (
     string Name ,
     string? ImageUrl ,
     bool IsPremium ,
     Goal Goal,
     int WorkoutDaysPerWeek
    );

    public class GetFilteredWorkoutPlansQueryHandler 
        : IRequestHandler<GetFilteredWorkoutPlansQuery, RequestResult<FilterPlansResultDto>>
    {
        private readonly GeneralRepository<Domain.Entities.WorkoutPlan> _workoutPlanRepository;

        public GetFilteredWorkoutPlansQueryHandler(GeneralRepository<Domain.Entities.WorkoutPlan> workoutPlanRepository)
        {
            _workoutPlanRepository = workoutPlanRepository;
        }

        public async Task<RequestResult<FilterPlansResultDto>> Handle(GetFilteredWorkoutPlansQuery request, CancellationToken cancellationToken)
        {
            var predicate = BuildPredicate(request);
            var Plans = _workoutPlanRepository.GetAll();
            var (data, total, totalPages) = await Plans
                .Where(predicate)
                .Select(p => new GetPlansDto(
                    p.Name,
                    p.ImageUrl,
                    p.IsPremium,
                    p.Goal,
                    p.WorkoutDaysPerWeek))
                .ToPaginatedAsync(request.PageIndex, request.PageSize, cancellationToken);

            var result = new FilterPlansResultDto(request.PageIndex, request.PageSize, data);

            return RequestResult<FilterPlansResultDto>.Success(result);
        }

        private static Expression<Func<Domain.Entities.WorkoutPlan, bool>> BuildPredicate(GetFilteredWorkoutPlansQuery query)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.WorkoutPlan>(true);

            if (!string.IsNullOrWhiteSpace(query.Name))
                predicate = predicate.And(p => p.Name.Contains(query.Name));

            if (query.Goal.HasValue)
                predicate = predicate.And(p => p.Goal == query.Goal.Value);

            if (query.WorkoutDaysPerWeek.HasValue)
                predicate = predicate.And(p => p.WorkoutDaysPerWeek == query.WorkoutDaysPerWeek.Value);

            return predicate;
        }
    }
}
