using MediatR;
using WorkoutService.Features.Common.Helpers;

namespace WorkoutService.Features.WorkoutPlan.GetWorkoutPlansList
{
    public record GetAllPlanQuery(
        int PageIndex,
        int PageSize
    ) : IRequest<RequestResult<AllPlansResultDto>>;

    public record AllPlansResultDto
    (
        int Page,
        int PageSize,
        IEnumerable<GetAllPlansDto> AllPlans
    );

    public record GetAllPlansDto
    {
    }
}
