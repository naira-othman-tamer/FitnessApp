using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.GetActiveAssignedUserPlan
{
    public record GetActiveUserPlanQuery(Guid userId) : IRequest<RequestResult<ActivePlanDto>>;

    public record ActivePlanDto(int planId , string WorkoutPlan="" , string NutritionPlan ="");

    public class GetActiveUserPlanQueryHandler : IRequestHandler<GetActiveUserPlanQuery, RequestResult<ActivePlanDto>>
    {
        private readonly GeneralRepository<UserAssignedPlan> _planRepo;

        public GetActiveUserPlanQueryHandler(GeneralRepository<UserAssignedPlan> planRepo)
        {
            _planRepo = planRepo;
        }

        public async Task<RequestResult<ActivePlanDto>> Handle(GetActiveUserPlanQuery request, CancellationToken cancellationToken)
        {
            var plan = await _planRepo
                .Get(u => u.userId == request.userId && u.IsActive==true)
                .Select(p => new ActivePlanDto
                (
                    p.Id,
                    p.WorkoutPlan,
                    p.NutritionPlan
                 )).FirstOrDefaultAsync(cancellationToken);

            return RequestResult<ActivePlanDto>.Success(plan);
        }
    }

    public static class GetPlanEndPoint
    {
        public static void MapGetActiveUserPlanEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/{userID}", async (
               Guid userID,
               IMediator mediator
               ) =>
            {
                var userPlan = await mediator.Send(new GetActiveUserPlanQuery(userID));
                return Results.Ok(userPlan);
            });
        }

    }
}
