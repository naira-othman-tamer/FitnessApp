using FCE.Domain.Aggregates;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            if (request.metrics is null)
            {
                return RequestResult<bool>.Failure("Metrics are null", RequestErrorCode.InvalidMetricsInput);
            }

            var existingMetrics = await _metricsRepo
                .Get(x => x.UserId == request.metrics.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingMetrics is null)
            {
                _metricsRepo.Add(request.metrics);
            }
            else
            {
                existingMetrics.UpdateFrom(request.metrics);
                _metricsRepo.Update(existingMetrics);
            }

            await _metricsRepo.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }
}
