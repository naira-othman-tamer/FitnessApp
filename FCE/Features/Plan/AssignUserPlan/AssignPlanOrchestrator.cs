using ContractMessages.Enums;
using ContractMessages.WorkoutPlanMatching;
using FCE.Features.Metrics.GetUserCurrentMetrics;
using FCE.Features.Stats.GetUsetStats;
using MassTransit;
using MassTransit.Initializers;
using MediatR;
using static MassTransit.Transports.ReceiveEndpoint;

namespace FCE.Features.Plan.AssignUserPlan
{
    public record AssignPlanOrchestrator(Guid userId) : IRequest<bool>;

    public class AssignPlanOrchestratorHandler : IRequestHandler<AssignPlanOrchestrator, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRequestClient<IGetWorkoutPlanRequest> _workoutPlanClient;

        public AssignPlanOrchestratorHandler(IMediator mediator, IRequestClient<IGetWorkoutPlanRequest> workoutPlanClient)
        {
            _mediator = mediator;
            _workoutPlanClient = workoutPlanClient;
        }

        public async Task<bool> Handle(AssignPlanOrchestrator request, CancellationToken cancellationToken)
        {
            var stats = await _mediator
                .Send(new GetUsetStatsQuery(request.userId), cancellationToken);

            var metrics = await _mediator
                .Send(new GetUserMetricsQuery(request.userId), cancellationToken);
                

            var workoutResponse = await _workoutPlanClient.GetResponse<IGetWorkoutPlanResponse>(
              new
              {
                  Goal = stats.userGoal,
                  WorkoutDaysPerWeek = stats.WorkoutDays  
              },
              cancellationToken
          );
            string workoutPlanName = workoutResponse.Message.Name;

            await _mediator.Send(new AssignUserPlanCommand
                (
                request.userId,
                stats.userGoal,
                metrics.CalorieTarget,
                workoutPlanName,
                ""
                ), cancellationToken);



            return true;
        }
    }
}
