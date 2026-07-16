using FCE.Domain.Aggregates;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics.SetUserCalculatedMetrics.Queries
{
    public record CheckUserHasMetricsQuery (Guid userId) : IRequest<RequestResult<bool>>;

    public class CheckMetricsQueryHandler : IRequestHandler<CheckUserHasMetricsQuery, RequestResult<bool>>
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;
        public CheckMetricsQueryHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }
        public async Task<RequestResult<bool>> Handle(CheckUserHasMetricsQuery request, CancellationToken cancellationToken)
        {
            var isExist = await _metricsRepo
                .Get(m => m.UserId == request.userId)
                .AnyAsync(cancellationToken);

            if (isExist)
            {
                return RequestResult<bool>
                    .Failure("Failed to retrieve user metrics.",
                    RequestErrorCode.UserMetricsNotFound);
            }
            
      
            return RequestResult<bool>.Success(true);
        }
    }

}
