using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;

namespace FCE.Features.Plan
{
    public record SetUserPlanHistoryCommand 
       ( Guid userId,
         int externalPlanId,
         string reason ,
         DateTime planCreationTime
       ) : ICommand<bool>;

    public class SetUserPlanHistoryCommandHandler : IRequestHandler<SetUserPlanHistoryCommand, bool>
    {
        private readonly GeneralRepository<UserPlanHistory> _planHistoryRepo;

        public SetUserPlanHistoryCommandHandler(GeneralRepository<UserPlanHistory> planHistoryRepo)
        {
            _planHistoryRepo = planHistoryRepo;
        }

        public async Task<bool> Handle(SetUserPlanHistoryCommand request, CancellationToken cs)
        {
            _planHistoryRepo.Add(new UserPlanHistory
            {
                UserId = request.userId,
                ResonForChange = request.reason,
                ExternalPlanId = request.externalPlanId,
                CreatedAt = request.planCreationTime,
                EndedAt = DateTime.UtcNow,
            });
           
            await _planHistoryRepo.SaveChangesAsync();

            return true;
        }
    }
}
