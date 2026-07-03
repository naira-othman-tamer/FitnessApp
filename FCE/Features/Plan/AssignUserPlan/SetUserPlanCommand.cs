using ContractMessages.Enums;
using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.Plan.AssignUserPlan
{
    public record SetUserPlanCommand(
        Guid userId,
        Goal userGoal,
        double IntakeClaorie,
        string WorkoutPlanName,
        string NutritionPlanName) : ICommandRequest<RequestResult<int>>;

    public class AssignUserPlanCommandHandler : IRequestHandler<SetUserPlanCommand, RequestResult<int>>
    {
        private readonly GeneralRepository<UserAssignedPlan> _userPlanRepo;

        public AssignUserPlanCommandHandler(GeneralRepository<UserAssignedPlan> userPlanRepo)
        {
            _userPlanRepo = userPlanRepo;
        }

        public async Task<RequestResult<int>> Handle(SetUserPlanCommand request, CancellationToken cancellationToken)
        {
           var plan = UserAssignedPlan
                .Create(request.userId , request.userGoal ,request.IntakeClaorie, request.WorkoutPlanName , request.NutritionPlanName);
            
            _userPlanRepo.Add(plan);
           await _userPlanRepo.SaveChangesAsync();

           return RequestResult<int>.Success(plan.Id);
        }
    }
}
