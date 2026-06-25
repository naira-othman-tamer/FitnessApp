using FCE.Domain.Aggregates;
using FCE.Domain.Entities;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics
{
    public record CalculateUserMetricsRequest(Guid userId) : IRequest<CalculatedMetrics> ;

    public class GetUserCurrentMetricsQueryHandler : IRequestHandler<CalculateUserMetricsRequest, CalculatedMetrics>
    {
        private readonly GeneralRepository<UserFitnessStats> _statesRepo;

        public GetUserCurrentMetricsQueryHandler(GeneralRepository<UserFitnessStats> statesRepo)
        {
            _statesRepo = statesRepo;
        }

        public async Task<CalculatedMetrics> Handle(CalculateUserMetricsRequest request, CancellationToken cancellationToken)
        {
            var stats = await _statesRepo
                .Get(u => u.userId == request.userId)
                .Select(s => new {
                    s.PhysicalStats,
                    s.activityLevel,
                    s.goal
                }).FirstOrDefaultAsync(cancellationToken);

            var metrics = CalculatedMetrics
                .Calculate(request.userId,stats.PhysicalStats,stats.activityLevel,stats.goal);

            return metrics;
        }
    }
}
