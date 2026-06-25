using FCE.Domain.Aggregates;
using FCE.Domain.ValueObject;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics
{
    public record GetUserMetricsQuery(Guid userId) : IRequest<MetabolicCalculator>;

    public class GetUserMetricsQueryHandler : IRequestHandler<GetUserMetricsQuery, MetabolicCalculator>
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public GetUserMetricsQueryHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<MetabolicCalculator> Handle(GetUserMetricsQuery request, CancellationToken cancellationToken)
        {
            var usermetrics = await _metricsRepo.Get(u => u.UserId == request.userId)
                .Select(m => m.Result)
                .FirstOrDefaultAsync(cancellationToken);

            return usermetrics;
        }
    }

}
