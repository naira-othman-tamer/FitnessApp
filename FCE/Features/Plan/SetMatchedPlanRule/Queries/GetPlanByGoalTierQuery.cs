using FCE.Domain.Entities;
using FCE.Domain.Enums;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.SetMatchedPlanRule.Queries
{
    public record GetPlanByGoalTierQuery(Goal goal, CalorieTarget tier) : IRequest<int>;

    public class GetPlanByGoalTierQueryHandler : IRequestHandler<GetPlanByGoalTierQuery, int>
    {
        private readonly GeneralRepository<TargetPlan> _planRuleRepo;

        public GetPlanByGoalTierQueryHandler(GeneralRepository<TargetPlan> planRuleRepo)
        {
            _planRuleRepo = planRuleRepo;
        }

        public async Task<int> Handle(GetPlanByGoalTierQuery request, CancellationToken cancellationToken)
        {
            var externalPlanRuleId = await _planRuleRepo
                .Get(p=>p.calorieIntake==request.tier && p.goal== request.goal)
                .Select(p=>p.ExternalPlanId)
                .FirstOrDefaultAsync(cancellationToken);

            return externalPlanRuleId;
        }
    }
}
