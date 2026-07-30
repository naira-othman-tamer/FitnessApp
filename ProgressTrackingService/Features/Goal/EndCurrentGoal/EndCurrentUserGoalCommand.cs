using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Common.Helpers;
using ProgressTrackingService.Infrastructure;

namespace ProgressTrackingService.Features.Goal.EndCurrentGoal;

public record EndCurrentUserGoalCommand(int ProgressGoalId) : ICommandRequest<RequestResult<bool>>;

public class EndCurrentUserGoalCommandHandler : IRequestHandler<EndCurrentUserGoalCommand, RequestResult<bool>>
{
    private readonly GeneralRepository<ProgressGoals> _progressRepo;

    public EndCurrentUserGoalCommandHandler(GeneralRepository<ProgressGoals> progressGoalsRepository)
    {
        _progressRepo = progressGoalsRepository;
    }

    public async Task<RequestResult<bool>> Handle(EndCurrentUserGoalCommand request, CancellationToken cancellationToken)
    {
        var CheckCurrentGoal = await _progressRepo
            .Get(p => p.Id == request.ProgressGoalId && p.CompletedAt == null && p.IsDeleted == false)
            .AnyAsync(cancellationToken);

        if (!CheckCurrentGoal)
        {
            return RequestResult<bool>.Failure("Invalid Current Goal" , RequestErrorCode.Conflict);
        }

        var updateProgressGoal = new ProgressGoals
        {
            Id = request.ProgressGoalId,
            CompletedAt = DateTime.UtcNow,
        };

        _progressRepo
            .UpdateInclude(updateProgressGoal, nameof(updateProgressGoal.CompletedAt));

        await _progressRepo.SaveChangesAsync(cancellationToken);

        return RequestResult<bool>.Success(true);
    }
}