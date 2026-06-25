using MediatR;

namespace FCE.Features.Metrics
{
    public record SubmitCalculatedMetricsOrchestrator(Guid userId) : IRequest<bool>;
    public class SubmitCalculatedMetricsOrchestratorHandler : IRequestHandler<SubmitCalculatedMetricsOrchestrator, bool>
    {
        private readonly IMediator _mediator;

        public SubmitCalculatedMetricsOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> Handle(SubmitCalculatedMetricsOrchestrator request, CancellationToken cancellationToken)
        {
            var metrics = await _mediator.Send(new CalculateUserMetricsRequest(request.userId), cancellationToken);
            await _mediator.Send(new SetMetricsCommand(metrics), cancellationToken);
            return true;
        }
    }

    public static class CalculateMetricsEndPoint
    {
        public static void MapSubmitCalculateMetricsEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("stats", async (
                SubmitCalculatedMetricsOrchestrator request,
                IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Ok(result);
            });
        }
    }

}
