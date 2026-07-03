using ContractMessages.Enums;
using MediatR;

namespace WorkoutService.Features.Plan.MatchUserWorkoutPlan
{
    public record MatchWorkoutPlanOrchestrator(Goal userGoal , int workoutDays) : IRequest<MatchedWorkoutPlanDTO>;

    public record MatchedWorkoutPlanDTO(int Id, string Name);

    public class MatchWorkoutPlanOrchestratorHandler : IRequestHandler<MatchWorkoutPlanOrchestrator, MatchedWorkoutPlanDTO>
    {
        public async Task<MatchedWorkoutPlanDTO> Handle(MatchWorkoutPlanOrchestrator request, CancellationToken cancellationToken)
        {
            return new MatchedWorkoutPlanDTO(3, "workputPlan");
        }
    }
}
