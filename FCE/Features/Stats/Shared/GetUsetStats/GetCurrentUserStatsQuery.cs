using ContractMessages.Enums;
using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Stats.Shared.GetUsetStats
{
    public record GetCurrentUserStatsQuery(Guid userId) : IRequest<RequestResult<GetUserStatsDTO>>;

    public class GetCurrentUserStatsQueryValidator : AbstractValidator<GetCurrentUserStatsQuery>
    {
        public GetCurrentUserStatsQueryValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty()
                .WithMessage("UserId is required");
        }
    }

    public record GetUserStatsDTO
        (
        ActivityLevel userActivelvl,
        Goal userGoal,
        double userWeight,
        double userHeight,
        short userAge,
        int WorkoutDays
        );
    public class GetCurrentUserStatsQueryHandler : IRequestHandler<GetCurrentUserStatsQuery, RequestResult<GetUserStatsDTO>>
    {
        private readonly GeneralRepository<UserFitnessStats> _statsRepository;

        public GetCurrentUserStatsQueryHandler(GeneralRepository<UserFitnessStats> statsRepository)
        {
            _statsRepository = statsRepository;
        }

        public async Task<RequestResult<GetUserStatsDTO>> Handle(GetCurrentUserStatsQuery request, CancellationToken cs)
        {
            var userStats = await _statsRepository
                .Get(s => s.userId == request.userId)
                .Select(s => new GetUserStatsDTO
                (
                   s.activityLevel,
                   s.goal,
                   s.PhysicalStats.Weight,
                   s.PhysicalStats.Height,
                   s.PhysicalStats.Age,
                   s.WorkoutDays
                )).FirstOrDefaultAsync(cs);

            if (userStats is null)
            {
                return RequestResult<GetUserStatsDTO>.Failure("User stats not found", RequestErrorCode.NotFound);
            }

            return RequestResult<GetUserStatsDTO>.Success(userStats);
        }
    }

    public static class GetStatsEndPont
    {
        public static void GetUserStatsEndPoint(this IEndpointRouteBuilder builder)
        {
            builder.MapGet("/{userID}", async (
               Guid userID,
               IMediator mediator
               ) =>
            {
                var userStatsResult = await mediator.Send(new GetCurrentUserStatsQuery(userID));
                if (!userStatsResult.IsSuccess)
                {
                    return Results.Problem(userStatsResult.Message, statusCode: userStatsResult.requestErrorCode == RequestErrorCode.NotFound ? 404 : 400);
                }
                return Results.Ok(userStatsResult.Data);
            });
        }
    }
}
