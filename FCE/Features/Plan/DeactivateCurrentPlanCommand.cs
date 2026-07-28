using FCE.Domain.Entities;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Plan;

public record DeactivateCurrentPlanCommand(Guid userId) : ICommandRequest<RequestResult<bool>>;

public class DeactivateCurrentPlanCommandHandler : IRequestHandler<DeactivateCurrentPlanCommand, RequestResult<bool>>
{
    private readonly GeneralRepository<UserAssignedPlan> _userAssignedPlanRepository;

    public DeactivateCurrentPlanCommandHandler(GeneralRepository<UserAssignedPlan> userAssignedPlanRepository)
    {
        _userAssignedPlanRepository = userAssignedPlanRepository;
    }

    public async Task<RequestResult<bool>> Handle(DeactivateCurrentPlanCommand request, CancellationToken cancellationToken)
    {
        var CurrentMetrics = await _userAssignedPlanRepository
           .Get(p => p.userId == request.userId)
           .FirstOrDefaultAsync(cancellationToken);

        if (CurrentMetrics is null)
        {
            return RequestResult<bool>.Failure("No active plan found for current user", RequestErrorCode.NotFound);
        }

        CurrentMetrics.IsActive = false;
        _userAssignedPlanRepository.UpdateInclude(CurrentMetrics, nameof(CurrentMetrics.IsActive));
        await _userAssignedPlanRepository.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true);
    }
}