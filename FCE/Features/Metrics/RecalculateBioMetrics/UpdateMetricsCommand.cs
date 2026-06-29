using FCE.Domain.Aggregates;
using FCE.Domain.ValueObject;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace FCE.Features.Metrics.RecalculateBioMetrics
{
    public record UpdateMetricsCommand(Guid userId, double weight) : ICommand<CalculatedMetrics>;

    public class UpdateMetricsCommandHandler : IRequestHandler<UpdateMetricsCommand, CalculatedMetrics>
    {
        private readonly GeneralRepository<UserFitnessStats> _statsRepo;

        public UpdateMetricsCommandHandler(GeneralRepository<UserFitnessStats> statsRepo)
        {
            _statsRepo = statsRepo;
        }

        public async Task<CalculatedMetrics> Handle(UpdateMetricsCommand request, CancellationToken cancellationToken)
        {
          
            var CurrentPhysicalStats = await _statsRepo
                .Get(u=>u.userId==request.userId)
                .Select(s=>new { s.PhysicalStats, s.Id })
                .FirstOrDefaultAsync(cancellationToken);
        
        var UpdatedStats = new UserFitnessStats
            {
                Id = CurrentPhysicalStats.Id,
                userId = request.userId,
                PhysicalStats = CurrentPhysicalStats.PhysicalStats with { Weight = request.weight }
            };
            _statsRepo.UpdateInclude(UpdatedStats, nameof(PhysicalStats));
            await _statsRepo.SaveChangesAsync();
            return CalculatedMetrics.Calculate(UpdatedStats);
        }
    }

    public static class CalculateMetricsEndPoint
    {
        public static void UpdateCalculateMetricsEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("/UpdateWeight", async (
                [FromBody] UpdateMetricsCommand request,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Ok(result);
            });
        }
    }


}
