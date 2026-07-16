using FCE.Features.Common.Helpers;
using FCE.Features.Metrics.SetUserCalculatedMetrics.Commands;
using FCE.Features.Metrics.SetUserCalculatedMetrics.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCE.Features.Metrics.SetUserCalculatedMetrics.Orchestrator
{
    public record SubmitCalculatedMetricsOrchestrator(Guid userId) : IRequest<RequestResult<bool>>;

    public class SubmitCalculatedMetricsOrchestratorValidator : AbstractValidator<SubmitCalculatedMetricsOrchestrator>
    {
        public SubmitCalculatedMetricsOrchestratorValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }

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
            if (!metrics.IsSuccess)
            {
                return RequestResult<bool>
                    .Failure(metrics.Message?? "Failed to calculate user metrics.", metrics.requestErrorCode?? RequestErrorCode.CalculationFailed);
            }
        
            var IsuserHasMetrics = await _mediator.Send(new CheckUserHasMetricsQuery(request.userId), cancellationToken);
            if (!IsuserHasMetrics.IsSuccess)
            {
                return RequestResult<bool>
                    .Failure(IsuserHasMetrics.Message?? "Failed to check user metrics.",
                    IsuserHasMetrics.requestErrorCode?? RequestErrorCode.CalculationFailed);
            }
            var setResult = await _mediator.Send(new SetMetricsCommand(metrics.Data!), cancellationToken);
            if (!setResult.IsSuccess)
            {
                return RequestResult<bool>
                    .Failure(setResult.Message?? "Failed to set user metrics.", setResult.requestErrorCode?? RequestErrorCode.CalculationFailed);
            }
            return RequestResult<bool>.Success(true);
        }
    }

    public static class CalculateMetricsEndPoint
    {
        public static void MapSubmitCalculateMetricsEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
                [FromBody] SubmitCalculatedMetricsOrchestrator request, 
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                if (!result.IsSuccess)
                {
                    return Results.BadRequest(new { result.Message, result.requestErrorCode });
                }
                return Results.Ok(result.Data);
            });
        }
    }

}
