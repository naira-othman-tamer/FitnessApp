using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.Plan
{
    public record AssignUserPlanCommand(Guid userId, int ExternalplanId) : ICommand<bool>;

    public class AssignUserPlanCommandHandler : IRequestHandler<AssignUserPlanCommand, bool>
    {
        private readonly GeneralRepository<UserAssignedPlan> _assignedPlanRepo;

        public AssignUserPlanCommandHandler(GeneralRepository<UserAssignedPlan> assignedPlanRepo)
        {
            _assignedPlanRepo = assignedPlanRepo;
        }

        public async Task<bool> Handle(AssignUserPlanCommand request, CancellationToken cancellationToken)
        {
            var newPlan = new UserAssignedPlan
            {
                userId = request.userId,
                ExternalPlanId = request.ExternalplanId,
                CreatedAt = DateTime.Now
            };
            _assignedPlanRepo.Add(newPlan);
            await _assignedPlanRepo.SaveChangesAsync();
            return true;
        }
    }
}
