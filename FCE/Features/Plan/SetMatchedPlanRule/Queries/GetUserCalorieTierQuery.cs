using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan.SetMatchedPlanRule.Queries
{
    public record GetUserCalorieTierQuery(Guid userId) : IRequest<BMRStatus>;

    public class GetUserCalorieTierQueryHandler : IRequestHandler<GetUserCalorieTierQuery, BMRStatus>
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public GetUserCalorieTierQueryHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<BMRStatus> Handle(GetUserCalorieTierQuery request, CancellationToken cancellationToken)
        {
           var tier = await _metricsRepo
                .Get(u => u.UserId == request.userId)
                .Select(m => m.Result.Tier)
                .FirstOrDefaultAsync(cancellationToken);

            return tier;
        }
    }

}
