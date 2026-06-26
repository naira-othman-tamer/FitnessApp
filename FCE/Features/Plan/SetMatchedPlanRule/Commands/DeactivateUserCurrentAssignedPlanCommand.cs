using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.Plan.SetMatchedPlanRule.Commands
{
    public record DeactivateUserCurrentAssignedPlanCommand(Guid userId) : ICommand<int>;

    public class DeactivateUserCurrentAssignedPlanCommandHandler : IRequestHandler<DeactivateUserCurrentAssignedPlanCommand, int>
    {
        private readonly GeneralRepository<UserAssignedPlan> _assignedPlanRepo;

        public DeactivateUserCurrentAssignedPlanCommandHandler(GeneralRepository<UserAssignedPlan> assignedPlanRepo)
        {
            _assignedPlanRepo = assignedPlanRepo;
        }

        public async Task<int> Handle(DeactivateUserCurrentAssignedPlanCommand request, CancellationToken cancellationToken)
        {
            var activePlan = new UserAssignedPlan
            {
                userId = request.userId,
                IsActive = false,
                UpdatedAt = DateTime.Now
            };

            _assignedPlanRepo.UpdateInclude(activePlan, nameof(activePlan.IsActive), nameof(activePlan.UpdatedAt));
            await _assignedPlanRepo.SaveChangesAsync();
            return activePlan.ExternalPlanId;
        }
    }

}
