using ContractMessages.NutritionPlanMatching;
using ContractMessages.WorkoutPlanMatching;
using FCE.Features.Common.Helpers;
using FCE.Features.Metrics.GetUserCurrentMetrics;
using FCE.Features.Stats.Shared.GetUsetStats;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCE.Features.Plan.AssignUserPlan
{
    public record AssignPlanOrchestrator(Guid userId) : IRequest<RequestResult<bool>>;

    public class AssignPlanOrchestratorValidator : AbstractValidator<AssignPlanOrchestrator>
    {
        public AssignPlanOrchestratorValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("User ID cannot be empty.");
        }
    }

    public class AssignPlanOrchestratorHandler : IRequestHandler<AssignPlanOrchestrator, RequestResult<bool>>
    {
        private readonly IMediator _mediator;
        private readonly IRequestClient<IGetWorkoutPlanRequest> _workoutPlanClient;
        private readonly IRequestClient<IGetNutritionPlanRequest> _nutritionPlanClient;

        public AssignPlanOrchestratorHandler(IMediator mediator, IRequestClient<IGetWorkoutPlanRequest> workoutPlanClient, IRequestClient<IGetNutritionPlanRequest> nutritionPlanClient)
        {
            _mediator = mediator;
            _workoutPlanClient = workoutPlanClient;
            _nutritionPlanClient = nutritionPlanClient;
        }

        public async Task<RequestResult<bool>> Handle(AssignPlanOrchestrator request, CancellationToken cancellationToken)
        {
            var stats = await _mediator
                .Send(new GetCurrentUserStatsQuery(request.userId), cancellationToken);

            if (!stats.IsSuccess)
            {
                return RequestResult<bool>.Failure(stats.Message ?? "Failed to retrieve user stats.", stats.requestErrorCode);
            }
            if (stats.Data is null)
            {
                return RequestResult<bool>.Failure("User stats response did not include data.", RequestErrorCode.UserStatsNotFound);
            }

            var metrics = await _mediator
                .Send(new GetUserMetricsQuery(request.userId), cancellationToken);

            if (!metrics.IsSuccess)
            {
                return RequestResult<bool>.Failure(metrics.Message ?? "Failed to retrieve user metrics.", metrics.requestErrorCode);
            }
            if (metrics.Data is null)
            {
                return RequestResult<bool>.Failure("User metrics response did not include data.", RequestErrorCode.GetUserMetricsFailed);
            }
            #region WorkoutPlanRequestClient

            var workoutResponse = await _workoutPlanClient.GetResponse<IGetWorkoutPlanResponse>(
            new
            {
                Goal = stats.Data.userGoal,
                WorkoutDaysPerWeek = stats.Data.WorkoutDays
            },
            cancellationToken
        );

            if (!workoutResponse.Message.IsSuccess)
            {
                return RequestResult<bool>.Failure(
                                          $"Workout plan matching failed: {workoutResponse.Message.ErrorCode}");
            }

            string workoutPlanName = workoutResponse.Message.WorkoutPlanName;
            int workoutPlanId = workoutResponse.Message.WorkoutPlanId; 
            #endregion

            var nutritionResponse = await _nutritionPlanClient.GetResponse<IGetNutritionPlanResponse>(
                new
                {
                    Goal = stats.Data.userGoal,
                    CalorieTarget = metrics.Data.CalorieTarget
                },
                cancellationToken
            );
            if (!nutritionResponse.Message.IsSuccess)
            {
                return RequestResult<bool>.Failure(
                                          $"Nutrition plan matching failed: {nutritionResponse.Message.ErrorCode}");
            }
            string nutritionPlanName = nutritionResponse.Message.NutritionPlanName;
            Guid nutritionPlanId = nutritionResponse.Message.NutritionPlanId;

            var setPlanResult = await _mediator.Send(new SetUserPlanCommand
                (
                request.userId,
                stats.Data.userGoal,
                metrics.Data.CalorieTarget,
                workoutPlanName,
                workoutPlanId,
                nutritionPlanName,
                nutritionPlanId
                ), cancellationToken);

            if (!setPlanResult.IsSuccess)
                return RequestResult<bool>.Failure(setPlanResult.Message ?? "Failed to assign plan.", setPlanResult.requestErrorCode);

            return RequestResult<bool>.Success(true);
        }
    }

    public static class AssignUserPlanEndPoint
    {
        public static void MapAssignUserPlanEndPointEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
                [FromBody] AssignPlanOrchestrator request,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return result.IsSuccess
                             ? Results.Ok(result.Data)
                             : Results.BadRequest(result.Data);
            });
        }
    }
}
