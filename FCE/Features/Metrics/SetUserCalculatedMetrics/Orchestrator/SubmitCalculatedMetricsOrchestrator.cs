using FCE.Features.Common.Helpers;
using FCE.Features.Metrics.SetUserCalculatedMetrics.Commands;
using FCE.Features.Metrics.SetUserCalculatedMetrics.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCE.Features.Metrics.SetUserCalculatedMetrics.Orchestrator
{
    public record SubmitCalculatedMetricsOrchestrator(Guid userId) : IRequest<RequestResult<bool>>;
    public class SubmitCalculatedMetricsOrchestratorHandler : IRequestHandler<SubmitCalculatedMetricsOrchestrator, RequestResult<bool>>
    {
        private readonly IMediator _mediator;

        public SubmitCalculatedMetricsOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResult<bool>> Handle(SubmitCalculatedMetricsOrchestrator request, CancellationToken cancellationToken)
        {
            var metrics = await _mediator.Send(new CalculateUserMetricsRequest(request.userId), cancellationToken);
            await _mediator.Send(new SetMetricsCommand(metrics), cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }

    public static class CalculateMetricsEndPoint
    {
        public static void SubmitCalculateMetricsEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
                [FromBody] SubmitCalculatedMetricsOrchestrator request, 
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return Results.Ok(result);
            });
        }
    }

}
