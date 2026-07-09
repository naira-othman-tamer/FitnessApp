using ContractMessages.WorkoutPlanMatching;
using FCE.Features.Common.Helpers;
using FCE.Features.Metrics.GetUserCurrentMetrics;
using FCE.Features.Stats.GetUsetStats;
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

        public AssignPlanOrchestratorHandler(IMediator mediator, IRequestClient<IGetWorkoutPlanRequest> workoutPlanClient)
        {
            _mediator = mediator;
            _workoutPlanClient = workoutPlanClient;
        }

        public async Task<RequestResult<bool>> Handle(AssignPlanOrchestrator request, CancellationToken cancellationToken)
        {
            var stats = await _mediator
                .Send(new GetCurrentUserStatsQuery(request.userId), cancellationToken);

            if (!stats.IsSuccess)
            {
                return RequestResult<bool>.Failure(stats.Message ?? "Failed to retrieve user stats.", stats.requestErrorCode);
            }

            var metrics = await _mediator
                .Send(new GetUserMetricsQuery(request.userId), cancellationToken);

            if (!metrics.IsSuccess)
            {
                return RequestResult<bool>.Failure(metrics.Message ?? "Failed to retrieve user metrics.", metrics.requestErrorCode);
            }

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

            var setPlanResult = await _mediator.Send(new SetUserPlanCommand
                (
                request.userId,
                stats.Data.userGoal,
                metrics.Data.CalorieTarget,
                workoutPlanName,
                workoutPlanId,
                "",
                null
                ), cancellationToken);

            if (!setPlanResult.IsSuccess)
                return RequestResult<bool>.Failure(setPlanResult.Message ?? "Failed to assign plan.", setPlanResult.requestErrorCode);

            return RequestResult<bool>.Success(true);
        }
    }

    public static class AssignUserPlanEndPoint
    {
        public static void AssignUserPlanEndPointEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapPost("", async (
                [FromBody] AssignPlanOrchestrator request,
                [FromServices] IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                return result.IsSuccess
                             ? Results.Ok(result)
                             : Results.BadRequest(result);
            });
        }
    }
}
