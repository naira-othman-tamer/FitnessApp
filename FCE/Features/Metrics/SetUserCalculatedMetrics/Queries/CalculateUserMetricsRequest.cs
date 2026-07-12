using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Metrics.SetUserCalculatedMetrics.Queries
{
    public record CalculateUserMetricsRequest(Guid userId) : IRequest<RequestResult<CalculatedMetrics>> ;

    public record CalculatedMetricsResponseDTO
    (
     double UserBMR,
     double UserTDEE,
     double UserCalorieTarget,
     BMRStatus UserBMRStatus,
     BMRRange UserBMRRange
    );

    public class CalculateUserMetricsRequestValidator : AbstractValidator<CalculateUserMetricsRequest>
    {
        public CalculateUserMetricsRequestValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }

    public class GetUserCurrentMetricsQueryHandler : IRequestHandler<CalculateUserMetricsRequest, RequestResult<CalculatedMetrics>>
    {
        private readonly GeneralRepository<UserFitnessStats> _statesRepo;

        public GetUserCurrentMetricsQueryHandler(GeneralRepository<UserFitnessStats> statesRepo)
        {
            _statesRepo = statesRepo;
        }

        public async Task<RequestResult<CalculatedMetrics>> Handle(CalculateUserMetricsRequest request, CancellationToken cancellationToken)
        {
            var stats = await _statesRepo
                .Get(u => u.userId == request.userId)
                .Select(s => new {
                    s.PhysicalStats,
                    s.activityLevel,
                    s.goal,
                    
                }).FirstOrDefaultAsync(cancellationToken);

            if (stats is null)
            {
                return RequestResult<CalculatedMetrics>.Failure("User stats not found.", RequestErrorCode.UserStatsNotFound);
            }

            var metrics = CalculatedMetrics
                .Calculate(request.userId,stats.PhysicalStats,stats.activityLevel,stats.goal,stats.PhysicalStats.Gender);

            if (metrics is null)
            {
                return RequestResult<CalculatedMetrics>.Failure("Failed to calculate user metrics.",RequestErrorCode.CalculationFailed);
            }

            //var result = new CalculatedMetricsResponseDTO
            //    (
            //        metrics.BMR,
            //        metrics.TDEE,
            //        metrics.CalorieTarget,
            //        metrics.BMRStatus,
            //        metrics.BMRRange
            //    );

            return RequestResult<CalculatedMetrics>.Success(metrics);
        }
    }
}
