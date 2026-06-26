using FCE.Domain.Entities;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.SetMatchedPlanRule.Queries
{
    public record CheckUserHasCurrentActivePlanQuery(Guid userId) : IRequest<bool>;

    public class CheckUserHasCurrentActivePlanQueryHandler : IRequestHandler<CheckUserHasCurrentActivePlanQuery, bool>
    {
        private readonly GeneralRepository<UserAssignedPlan> _assignedPlanRepo;

        public CheckUserHasCurrentActivePlanQueryHandler(GeneralRepository<UserAssignedPlan> assignedPlanRepo)
        {
            _assignedPlanRepo = assignedPlanRepo;
        }

        public async Task<bool> Handle(CheckUserHasCurrentActivePlanQuery request, CancellationToken cs)
        {
           var result = await _assignedPlanRepo
                .Get(p => p.userId == request.userId)
                .AnyAsync(p => p.IsActive == true,cs);

            return result;
        }
    }
}
