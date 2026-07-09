using FCE.Domain.Aggregates;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.Metrics.SetUserCalculatedMetrics.Commands
{ 
    public record SetMetricsCommand(CalculatedMetrics metrics) : ICommandRequest<RequestResult<bool>>;

    public class SetMetricsCommandHandler : IRequestHandler<SetMetricsCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public SetMetricsCommandHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<RequestResult<bool>> Handle(SetMetricsCommand request, CancellationToken cancellationToken)
        {
            _metricsRepo.Add(request.metrics);
            if (request.metrics is null)
            {
                return RequestResult<bool>.Failure("Metrics are null", RequestErrorCode.InvalidMetricsInput);
            }
            await _metricsRepo.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }
}
