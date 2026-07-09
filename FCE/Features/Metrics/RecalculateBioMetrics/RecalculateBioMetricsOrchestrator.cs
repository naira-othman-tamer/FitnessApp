using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using FCE.Features.Common.Helpers;
using FCE.Features.Metrics.GetUserCurrentMetrics;
using FCE.Features.Metrics.SetUserCalculatedMetrics.Orchestrator;
using FCE.Features.Stats.Shared.GetUsetStats;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCE.Features.Metrics.RecalculateBioMetrics
{
    public record RecalculateBioMetricsOrchestrator(Guid userId, double weight) : IRequest<RequestResult<UserMetabolicParametersDTO>>;

    public class RecalculateBioMetricsOrchestratorValidator : AbstractValidator<RecalculateBioMetricsOrchestrator>
    {
        public RecalculateBioMetricsOrchestratorValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("UserId is required");
            RuleFor(x => x.weight)
                .GreaterThan(0)
                .WithMessage("Weight must be greater than 0");
        }
    }

    public record UserMetabolicParametersDTO(
     double userBMR,
     double userTDEE,
     double userCalorieTarget,
     BMRStatus userBMRStatus,
     BMRRange userBMRRange);

    public class RecalculateBioMetricsOrchestratorHandler : IRequestHandler<RecalculateBioMetricsOrchestrator, RequestResult<UserMetabolicParametersDTO>>
    {
        private readonly IMediator _mediator;

        public RecalculateBioMetricsOrchestratorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<RequestResult<UserMetabolicParametersDTO>> Handle(RecalculateBioMetricsOrchestrator request, CancellationToken cancellationToken)
        {
            var setNewWeightResult = await _mediator
                .Send(new UpdateWeightCommand(request.userId, request.weight), cancellationToken);
            if (!setNewWeightResult.IsSuccess) {
                return RequestResult<UserMetabolicParametersDTO>
                    .Failure(setNewWeightResult.Message??"Failed to update weight.",
                    setNewWeightResult.requestErrorCode??RequestErrorCode.WeightUpdateFailed);
            }

            var CurrentUserStatsResult = await _mediator
                .Send(new GetCurrentUserStatsQuery(request.userId), cancellationToken);
            if (!CurrentUserStatsResult.IsSuccess || CurrentUserStatsResult.Data is null)
            {
                return RequestResult<UserMetabolicParametersDTO>
                    .Failure(CurrentUserStatsResult.Message ?? "Failed to retrieve user stats.",
                    CurrentUserStatsResult.requestErrorCode ?? RequestErrorCode.UserStatsNotFound);
            }

            var resetBioMetricsResult = await _mediator
                .Send(new SubmitCalculatedMetricsOrchestrator(request.userId), cancellationToken);
            if (!resetBioMetricsResult.IsSuccess) {
                return RequestResult<UserMetabolicParametersDTO>
                    .Failure(resetBioMetricsResult.Message ?? "Failed to reset bio metrics.",
                    resetBioMetricsResult.requestErrorCode ?? RequestErrorCode.BioMetricsResetFailed);
            }

            RequestResult<userMetricsDTO>? ReadMetricsResult = await _mediator
                .Send(new GetUserMetricsQuery(request.userId), cancellationToken);
            if (!ReadMetricsResult.IsSuccess) {
                return RequestResult<UserMetabolicParametersDTO>
                    .Failure(ReadMetricsResult.Message ?? "Failed to retrieve updated metrics.",
                    ReadMetricsResult.requestErrorCode ?? RequestErrorCode.GetUserMetricsFailed);
            }

            var result = new UserMetabolicParametersDTO(
                ReadMetricsResult.Data!.userBMR,
                ReadMetricsResult.Data.userTDEE,
                ReadMetricsResult.Data.CalorieTarget,
                ReadMetricsResult.Data.userTarget,
                ReadMetricsResult.Data.range);

            return RequestResult<UserMetabolicParametersDTO>.Success(result);
        }
    }

    public static class ReCalculateMetricsEndPoint
    {
        public static void ReCalculateMetrics(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
                [FromBody] RecalculateBioMetricsOrchestrator request,
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
