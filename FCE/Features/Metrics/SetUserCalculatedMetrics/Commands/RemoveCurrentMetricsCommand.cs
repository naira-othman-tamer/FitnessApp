using FCE.Domain.Aggregates;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics.SetUserCalculatedMetrics.Commands;

public record RemoveCurrentMetricsCommand(Guid userId) : ICommandRequest<RequestResult<bool>>;

public class RemoveCurrentMetricsCommandHandler : IRequestHandler<RemoveCurrentMetricsCommand, RequestResult<bool>>
{
    private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

    public RemoveCurrentMetricsCommandHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
    {
        _metricsRepo = metricsRepo;
    }

    public async Task<RequestResult<bool>> Handle(RemoveCurrentMetricsCommand request, CancellationToken cancellationToken)
    {
        var CurrentMetrics = await _metricsRepo
            .Get(m=>m.UserId==request.userId)
            .FirstOrDefaultAsync(cancellationToken);

        if(CurrentMetrics is null)
        {
            return RequestResult<bool>.Failure("No metrics Found for Current User", RequestErrorCode.NotFound);
        }

        _metricsRepo.SoftDelete(CurrentMetrics);
        await _metricsRepo.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true);
    }
}
