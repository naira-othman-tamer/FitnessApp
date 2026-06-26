using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.SetMatchedPlanRule.Queries
{
    public record GetUserGoalQuery(Guid userId) : IRequest<Goal>;

    public class GetUserGoalQueryHandler : IRequestHandler<GetUserGoalQuery, Goal>
    {
        private readonly GeneralRepository<UserFitnessStats> _statsRepo;

        public GetUserGoalQueryHandler(GeneralRepository<UserFitnessStats> statsRepo)
        {
            _statsRepo = statsRepo;
        }

        public async Task<Goal> Handle(GetUserGoalQuery request, CancellationToken cancellationToken)
        {
            var goal = await _statsRepo.Get(u => u.userId == request.userId)
                .Select(s => s.goal)
                .FirstOrDefaultAsync(cancellationToken);

            return goal;

        }
    }

}
