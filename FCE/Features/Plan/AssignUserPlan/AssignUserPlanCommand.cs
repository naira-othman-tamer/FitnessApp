using ContractMessages.Enums;
using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.Plan.AssignUserPlan
{
    public record AssignUserPlanCommand(Guid userId, Goal userGoal, double IntakeClaorie,string WorkoutPlanName,string NutritionPlanName) : ICommandRequest<int>;

    public class AssignUserPlanCommandHandler : IRequestHandler<AssignUserPlanCommand, int>
    {
        private readonly GeneralRepository<UserAssignedPlan> _userPlanRepo;

        public AssignUserPlanCommandHandler(GeneralRepository<UserAssignedPlan> userPlanRepo)
        {
            _userPlanRepo = userPlanRepo;
        }

        public async Task<int> Handle(AssignUserPlanCommand request, CancellationToken cancellationToken)
        {
           var plan = UserAssignedPlan
                .Create(request.userId , request.userGoal ,request.IntakeClaorie, request.WorkoutPlanName , request.NutritionPlanName);
            
            _userPlanRepo.Add(plan);
           await _userPlanRepo.SaveChangesAsync();

           return plan.Id;
        }
    }
}
