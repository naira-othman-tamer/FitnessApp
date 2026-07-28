using ContractMessages.NutritionPlanMatching;
using ContractMessages.WorkoutPlanMatching;
using FCE.Features.Common.Helpers;
using FCE.Features.Metrics.GetUserCurrentMetrics;
using FCE.Features.PlanHistory;
using FCE.Features.Stats.Shared.GetUsetStats;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCE.Features.Plan.AssignUserPlan;

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
        var HasUserActivePlan = await _mediator.Send(new CheckUserActivePlanQuery(request.userId), cancellationToken);
        if (HasUserActivePlan.IsSuccess)
        {
            //TODO => send message to archive Current Plan
            //var setPlanHistoryResult = await _mediator.Send(new SetUserPlanHistoryCommand(request.userId), cancellationToken);
            //if (!setPlanHistoryResult.IsSuccess)
            //{
            //    return RequestResult<bool>
            //        .Failure(setPlanHistoryResult.Message ?? "Failed to set user plan history.",
            //        setPlanHistoryResult.requestErrorCode);
            //}

            var deActiveCurrentPlanResult = await _mediator
                .Send(new DeactivateCurrentPlanCommand(request.userId),cancellationToken);

            if(!deActiveCurrentPlanResult.IsSuccess)
            {
                return RequestResult<bool>
               .Failure(deActiveCurrentPlanResult.Message ?? "Faied to remove Current Plan",
                        deActiveCurrentPlanResult.requestErrorCode ?? RequestErrorCode.Conflict);
            }

        }
        var currentStatesResult = await _mediator
            .Send(new GetCurrentUserStatsQuery(request.userId), cancellationToken);

        if (!currentStatesResult.IsSuccess)
        {
            return RequestResult<bool>
                .Failure(currentStatesResult.Message ?? "Failed to retrieve user currentStatesResult.",
                currentStatesResult.requestErrorCode ?? RequestErrorCode.NotFound);
        }

        var currentMetricsResult = await _mediator
            .Send(new GetUserMetricsQuery(request.userId), cancellationToken);

        if (!currentMetricsResult.IsSuccess)
        {
            return RequestResult<bool>.Failure(currentMetricsResult.Message ?? "Failed to retrieve user currentMetricsResult.", currentMetricsResult.requestErrorCode);
        }

        #region WorkoutPlanRequestClient

        var workoutResponse = await _workoutPlanClient.GetResponse<IGetWorkoutPlanResponse>(
        new
        {
            Goal = currentStatesResult.Data.userGoal,
            WorkoutDaysPerWeek = currentStatesResult.Data.WorkoutDays
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
                Goal = currentStatesResult.Data.userGoal,
                CalorieTarget = currentMetricsResult.Data.CalorieTarget
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
            currentStatesResult.Data.userGoal,
            currentMetricsResult.Data.CalorieTarget,
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
