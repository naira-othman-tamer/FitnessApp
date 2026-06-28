using FCE.Domain.Enums;
using FCE.Features.Plan.SetMatchedPlanRule.Commands;
using FCE.Features.Plan.SetMatchedPlanRule.Queries;
using MediatR;

namespace FCE.Features.Plan.SetMatchedPlanRule.Orchestrator
{
    public record MatchPlanRuleOrchestrator(Guid userId) : IRequest<bool>;

    public class MatchPlanRuleOrchestratorHandler : IRequestHandler<MatchPlanRuleOrchestrator, bool>
    {
        private readonly IMediator _mediator;

        public MatchPlanRuleOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(MatchPlanRuleOrchestrator request, CancellationToken cs)
        {
            Goal userGoal = await _mediator.Send(new GetUserGoalQuery(request.userId), cs);
            BMRStatus userTier = await _mediator.Send(new GetUserCalorieTierQuery(request.userId), cs);
            int matchedPlanExternalId = await _mediator.Send(new GetPlanByGoalTierQuery(userGoal, userTier), cs);

            if (await _mediator.Send(new CheckUserHasCurrentActivePlanQuery(request.userId), cs))
            {
                var inactiveExternalPlanId = await _mediator
                     .Send(new DeactivateUserCurrentAssignedPlanCommand(request.userId), cs);
                //create query request to get CreationalDate
                var recordHistoryPlan = await _mediator
                    .Send(new SetUserPlanHistoryCommand(request.userId, inactiveExternalPlanId, "", DateTime.Now), cs);
            }
            var assignNewUserPlan = await _mediator
                .Send(new AssignUserPlanCommand(request.userId, matchedPlanExternalId), cs);

            return true;
        }
    }

    public static class MatchPlanRuleEndPoint
    {
        public static void SetMatchedPlanRuleEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/{userId}", async (
                MatchPlanRuleOrchestrator request,
                IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Created();
            });
        }
    }
}
