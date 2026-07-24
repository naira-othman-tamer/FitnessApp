using MediatR;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Common.Helpers;
using ProgressTrackingService.Infrastructure;

namespace ProgressTrackingService.Features.Goal.UpdateUserGoalOrDateTarget;

public record UpdateUserProgressCommand(Guid userId, DateTime? TargetDate, double? TargetWeightKg)
    : ICommandRequest<RequestResult<bool>>;

public class UpdateUserProgressCommandHandler : IRequestHandler<UpdateUserProgressCommand, RequestResult<bool>>
{
    private readonly GeneralRepository<ProgressGoals> _progressRepo;

    public UpdateUserProgressCommandHandler(GeneralRepository<ProgressGoals> progressRepo)
    {
        _progressRepo = progressRepo;
    }

    public async Task<RequestResult<bool>> Handle(UpdateUserProgressCommand request, CancellationToken cancellationToken)
    {
        var updateProgressGoal = new ProgressGoals
        {
            UserId = request.userId,
            TargetDate = request?.TargetDate,
            TargetWeightKg = request?.TargetWeightKg
        };

        _progressRepo
            .UpdateInclude(updateProgressGoal, nameof(updateProgressGoal.TargetDate), nameof(updateProgressGoal.TargetWeightKg));

        await _progressRepo.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true);
    }
}