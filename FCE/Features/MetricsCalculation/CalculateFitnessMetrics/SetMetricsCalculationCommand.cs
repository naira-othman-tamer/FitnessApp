using FCE.Domain.Aggregates;
using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Features.UserStates.SubmitFitnessStats;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.MetricsCalculation.CalculateFitnessMetrics
{
    public record SetMetricsCalculationCommand(Guid userId) : ICommand<bool>;
    public class SetMetricsCalculationCommandHandler : IRequestHandler<SetMetricsCalculationCommand, bool>
    {
        private readonly GeneralRepository<UserFitnessStats> _statesRepo;
        private readonly GeneralRepository<CalculatedMetrics> _metricsRepo;

        public SetMetricsCalculationCommandHandler(GeneralRepository<UserFitnessStats> statesRepo, 
                                                  GeneralRepository<CalculatedMetrics> metricsRepo)
        {
            _statesRepo = statesRepo;
            _metricsRepo = metricsRepo;
        }

        public async Task<bool> Handle(SetMetricsCalculationCommand request, CancellationToken cancellationToken)
        {
            var userStats = _statesRepo.Get(u => u.userId == request.userId).FirstOrDefault();
            var metrics = CalculatedMetrics.Calculate(userStats);
            _metricsRepo.Add(metrics);
            await _metricsRepo.SaveChangesAsync();
            return true;
        }
    }

    public static class CalculateMetricsEndPoint
    {
        public static void MapCalculateMetricsEndPoints(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("stats", async (
                SetMetricsCalculationCommand request,
                IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Ok(result);
            });
        }
    }

}
