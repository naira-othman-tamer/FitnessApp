using ContractMessages.Enums;
using MediatR;

namespace WorkoutService.Features.Plan
{
    public record MatchWorkoutPlanOrchestrator(Goal userGoal , int workoutDays) : IRequest<MatchedWorkoutPlanDTO>;

    public record MatchedWorkoutPlanDTO(Guid Id, string Name);

    public class MatchWorkoutPlanOrchestratorHandler : IRequestHandler<MatchWorkoutPlanOrchestrator, MatchedWorkoutPlanDTO>
    {
        public async Task<MatchedWorkoutPlanDTO> Handle(MatchWorkoutPlanOrchestrator request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
