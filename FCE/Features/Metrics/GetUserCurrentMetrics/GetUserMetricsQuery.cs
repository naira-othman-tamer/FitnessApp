using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using FCE.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics.GetUserCurrentMetrics
{
    public record GetUserMetricsQuery(Guid userId) : IRequest<userMetricsDTO>;//MetabolicCalculator>;

    public record userMetricsDTO
    (
        double userBMR,
        double userTDEE,
        BMRRange range,
        BMRStatus userTarget
    );

    public class GetUserMetricsQueryHandler : IRequestHandler<GetUserMetricsQuery, userMetricsDTO> //, MetabolicCalculator>
    {
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public GetUserMetricsQueryHandler(GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _metricsRepo = metricsRepo;
        }

        public async Task<userMetricsDTO> Handle(GetUserMetricsQuery request, CancellationToken cancellationToken)
        {
            var usermetrics = await _metricsRepo.Get(u => u.UserId == request.userId)
                .Select(m => new userMetricsDTO
                (
                    m.BMR,
                    m.TDEE,
                    m.BMRRange,
                    m.BMRStatus
                ))
                .FirstOrDefaultAsync(cancellationToken);

            return usermetrics;
        }
    }

    public static class GetMetricsEndPoint

    {
        public static void GetUserMetricsEndpoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/{userId}", async ([FromQuery] Guid userId,
               [FromServices] IMediator mediator) =>
            {
                var userResult = await mediator.Send(new GetUserMetricsQuery(userId));
                return Results.Ok(userResult);
            });
        }
    }

}
