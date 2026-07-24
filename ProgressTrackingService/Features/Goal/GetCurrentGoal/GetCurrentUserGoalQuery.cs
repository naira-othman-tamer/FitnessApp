using MediatR;
using ProgressTrackingService.Domain.Entities;
using ProgressTrackingService.Features.Common.Helpers;
using ProgressTrackingService.Infrastructure;
using System.Data.Entity;

namespace ProgressTrackingService.Features.Goal.GetCurrentGoal;

public record GetCurrentUserGoalQuery(Guid userId) : IRequest<RequestResult<currentUserGoalDto>>;

public record currentUserGoalDto
(
 int CurrentGoalId,
 double? TargetWeightKg,
 DateTime? TargetDate,
 DateTime StartedAt
);

public class GetCurrentUserGoalQueryHandler : IRequestHandler<GetCurrentUserGoalQuery, RequestResult<currentUserGoalDto>>
{
    private readonly GeneralRepository<ProgressGoals> _progressRepo;

    public GetCurrentUserGoalQueryHandler(GeneralRepository<ProgressGoals> progressRepo)
    {
        _progressRepo = progressRepo;
    }

    public async Task<RequestResult<currentUserGoalDto>> Handle(GetCurrentUserGoalQuery request, CancellationToken cancellationToken)
    {
        var userProgressGoalDto = await _progressRepo
            .Get(p => p.UserId == request.userId && p.CompletedAt == null && p.IsDeleted == false)
            .Select(p => new currentUserGoalDto
            (
                p.Id,
                p.TargetWeightKg,
                p.TargetDate,
                p.CreatedAt
            )).FirstOrDefaultAsync(cancellationToken);

        if (userProgressGoalDto is null)
        {
            return RequestResult<currentUserGoalDto>.Failure("", RequestErrorCode.NotFound);
        }

        return RequestResult<currentUserGoalDto>.Success(userProgressGoalDto);
    }
}