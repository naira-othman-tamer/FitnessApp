using ContractMessages.Enums;
using FCE.Domain.Aggregates;
using FCE.Domain.Enums;
using FCE.Features.Common.Helpers;
using FCE.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FCE.Features.Stats.GetUsetStats
{
    public record GetUsetStatsQuery(Guid userId) : IRequest<RequestResult<GetUserStatsDTO>>;

    public record GetUserStatsDTO
        (
        ActivityLevel userActivelvl,
        Goal userGoal,
        double userWeight,
        double userHeight,
        short userAge,
        int WorkoutDays
        );
    public class GetUsetStatsQueryHandler : IRequestHandler<GetUsetStatsQuery, RequestResult<GetUserStatsDTO>>
    {
        private readonly GeneralRepository<UserFitnessStats> _statsRepository;

        public GetUsetStatsQueryHandler(GeneralRepository<UserFitnessStats> statsRepository)
        {
            _statsRepository = statsRepository;
        }

        public async Task<RequestResult<GetUserStatsDTO>> Handle(GetUsetStatsQuery request, CancellationToken cs)
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
                var userStats = await mediator.Send(new GetUsetStatsQuery(userID));
                return Results.Ok(userStats);
            });
        }
    }
}
