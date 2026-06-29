using FCE.Domain.Aggregates;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.Metrics.SetUserCalculatedMetrics.Commands
{ 
    public record SetMetricsCommand(CalculatedMetrics metrics) : ICommand<bool>;

    public class SetMetricsCommandHandler : IRequestHandler<SetMetricsCommand, bool>
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public SetMetricsCommandHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<bool> Handle(SetMetricsCommand request, CancellationToken cancellationToken)
        {
            _metricsRepo.Add(request.metrics);
            await _metricsRepo.SaveChangesAsync();
            return true;
        }
    }
}
