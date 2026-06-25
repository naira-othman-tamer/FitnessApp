using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan
{
    public record GetUserCalorieTierQuery(Guid userId) : IRequest<CalorieIntensityTier>;

    public class GetUserCalorieTierQueryHandler : IRequestHandler<GetUserCalorieTierQuery, CalorieIntensityTier>
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public GetUserCalorieTierQueryHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<CalorieIntensityTier> Handle(GetUserCalorieTierQuery request, CancellationToken cancellationToken)
        {
           var tier = await _metricsRepo
                .Get(u => u.UserId == request.userId)
                .Select(m => m.Result.Tier)
                .FirstOrDefaultAsync(cancellationToken);

            return tier;
        }
    }

}
