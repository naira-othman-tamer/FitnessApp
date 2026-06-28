using FCE.Domain.Entities;
using FCE.Features.Stats.SubmitFitnessStats;
using FCE.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.GetUserMatchedPlan
{
    public record GetMatchedActivePlanQuery(Guid userId) : IRequest<GetMatchedPlanDto>;

    public record GetMatchedPlanDto
        (
        int TargetPlanId,
        int ExternalPlanId
        //string TargetPlanName
        );
    public class GetMatchedActivePlanQueryHandler : IRequestHandler<GetMatchedActivePlanQuery, GetMatchedPlanDto>
    {
        private readonly GeneralRepository<UserAssignedPlan> _userAssignedPlanRepository;

        public GetMatchedActivePlanQueryHandler(GeneralRepository<UserAssignedPlan> userAssignedPlanRepository)
        {
            _userAssignedPlanRepository = userAssignedPlanRepository;
        }

        public async Task<GetMatchedPlanDto> Handle(GetMatchedActivePlanQuery request, CancellationToken cancellationToken)
        {
            var result = await _userAssignedPlanRepository
                .Get(u=> u.userId == request.userId && u.IsActive==true)
                .Select(p=>new GetMatchedPlanDto
                (
                    p.Id,
                    p.ExternalPlanId
                ))
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }

    public static class SubmitFitnessEndPoint
    {
        public static void GetMatchedUserActivePlanEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/{userId}", async ( [FromQuery] Guid userId,
               [FromServices] IMediator mediator) =>
            {
                var id = await mediator.Send(new GetMatchedActivePlanQuery(userId));
                return Results.Ok(id);
            });
        }
    }
}
