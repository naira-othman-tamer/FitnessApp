using ContractMessages.Enums;
using FluentValidation;
using MediatR;
using WorkoutService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using WorkoutService.Features.Common.Helpers;

namespace WorkoutService.Features.WorkoutPlan.MatchUserWorkoutPlan
{
    public record MatchWorkoutPlanOrchestrator(Goal userGoal , int workoutDays) : IRequest<RequestResult<MatchedWorkoutPlanDTO>>;

    public record MatchedWorkoutPlanDTO(int Id, string Name);

    public class MatchWorkoutPlanOrchestratorValidator : AbstractValidator<MatchWorkoutPlanOrchestrator>
    {
        public MatchWorkoutPlanOrchestratorValidator()
        {
            RuleFor(x => x.userGoal).IsInEnum();
            RuleFor(x => x.workoutDays).GreaterThan(0);
        }
    }

    public class MatchWorkoutPlanOrchestratorHandler : IRequestHandler<MatchWorkoutPlanOrchestrator, RequestResult<MatchedWorkoutPlanDTO>>
    {
        private readonly GeneralRepository<Domain.Entities.WorkoutPlan> _workoutPlanRepository;
        public MatchWorkoutPlanOrchestratorHandler(GeneralRepository<Domain.Entities.WorkoutPlan> workoutPlanRepository)
        {
            _workoutPlanRepository = workoutPlanRepository;
        }

        public async Task<RequestResult<MatchedWorkoutPlanDTO>> Handle(MatchWorkoutPlanOrchestrator request, CancellationToken cancellationToken)
        {
            var matchedPlan = await _workoutPlanRepository
                .Get(p => p.Goal == request.userGoal && p.WorkoutDaysPerWeek == request.workoutDays)
                .Select(p => new MatchedWorkoutPlanDTO(p.Id, p.Name))
                .FirstOrDefaultAsync(cancellationToken);

            if (matchedPlan is null)
            {
                return RequestResult<MatchedWorkoutPlanDTO>.Failure("No matching workout plan found.");
            }
            return RequestResult<MatchedWorkoutPlanDTO>.Success(matchedPlan);
        }
    }
}
