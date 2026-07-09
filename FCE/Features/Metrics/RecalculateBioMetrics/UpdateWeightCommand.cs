using FCE.Domain.Aggregates;
using FCE.Domain.ValueObject;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics.RecalculateBioMetrics
{
    public record UpdateWeightCommand(Guid userId , double newWeight) : ICommandRequest<RequestResult<bool>>;

    public class UpdateWeightCommandValidator : AbstractValidator<UpdateWeightCommand>
    {
        public UpdateWeightCommandValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.newWeight).GreaterThan(0).WithMessage("New weight must be greater than zero.");
        }
    }

    public class UpdateWeightCommandHandler : IRequestHandler<UpdateWeightCommand, RequestResult<bool>>
    {
        private readonly GeneralRepository<UserFitnessStats> _statsRepo;
        public UpdateWeightCommandHandler(GeneralRepository<UserFitnessStats> userStatsRepository)
        {
            _statsRepo = userStatsRepository;
        }
        public async Task<RequestResult<bool>> Handle(UpdateWeightCommand request, CancellationToken cancellationToken)
        {
            var CurrentPhysicalStats = await _statsRepo
                 .Get(u => u.userId == request.userId)
                 .Select(s => new { s.PhysicalStats, s.Id })
                 .FirstOrDefaultAsync(cancellationToken);

            if (CurrentPhysicalStats is null)
            {
                return RequestResult<bool>.Failure("User not found.", RequestErrorCode.UserStatsNotFound);
            }

            var UpdatedStats = new UserFitnessStats
            {
                Id = CurrentPhysicalStats.Id,
                userId = request.userId,
                PhysicalStats = CurrentPhysicalStats.PhysicalStats with { Weight = request.newWeight }
            };
            _statsRepo.UpdateInclude(UpdatedStats, nameof(PhysicalStats));
            await _statsRepo.SaveChangesAsync(cancellationToken);
            return RequestResult<bool>.Success(true);
        }
    }

}
