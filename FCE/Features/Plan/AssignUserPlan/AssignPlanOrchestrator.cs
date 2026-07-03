using ContractMessages.Enums;
using ContractMessages.WorkoutPlanMatching;
using FCE.Features.Common.Helpers;
using FCE.Features.Metrics.GetUserCurrentMetrics;
using FCE.Features.Stats.GetUsetStats;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCE.Features.Plan.AssignUserPlan
{
    public record AssignPlanOrchestrator(Guid userId) : IRequest<RequestResult<bool>>;

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
                .Send(new GetUsetStatsQuery(request.userId), cancellationToken);

            var metrics = await _mediator
                .Send(new GetUserMetricsQuery(request.userId), cancellationToken);
                

            var workoutResponse = await _workoutPlanClient.GetResponse<IGetWorkoutPlanResponse>(
              new
              {
                  Goal = stats.Data.userGoal,
                  WorkoutDaysPerWeek = stats.Data.WorkoutDays  
              },
              cancellationToken
          );
            string workoutPlanName = workoutResponse.Message.Name;

            await _mediator.Send(new SetUserPlanCommand
                (
                request.userId,
                stats.Data.userGoal,
                metrics.Data.CalorieTarget,
                workoutPlanName,
                ""
                ), cancellationToken);

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
                return Results.Ok(result);
            });
        }
    }
}
