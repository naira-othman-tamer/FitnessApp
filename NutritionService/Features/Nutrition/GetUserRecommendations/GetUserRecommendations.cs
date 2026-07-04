using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Infrastructure;

namespace NutritionService.Features.Nutrition.GetUserRecommendations;

public sealed record GetUserRecommendationsQuery(Guid UserId) : IRequest<OperationResult<UserRecommendationDto>>;

public sealed class GetUserRecommendationsHandler(NutritionDbContext context, IFceClient fce, IClaimsManager claims)
    : IRequestHandler<GetUserRecommendationsQuery, OperationResult<UserRecommendationDto>>
{
    public async Task<OperationResult<UserRecommendationDto>> Handle(GetUserRecommendationsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId != claims.UserId && !claims.IsInRole("Admin"))
            return OperationResultFactory.ServiceEligibleError<UserRecommendationDto>(default!, "You cannot request another user's recommendations.", "لا يمكنك طلب توصيات مستخدم آخر");

        var target = await fce.GetCalorieTargetAsync(request.UserId, cancellationToken);
        if (target is null)
            return OperationResultFactory.Error(default(UserRecommendationDto)!, "FCE calorie target is currently unavailable.", "هدف السعرات غير متاح حاليا", StatusCode.ServiceUnavailable);

        var plans = await context.MealPlans.AsNoTracking().AsSplitQuery().Include(x => x.Items).ThenInclude(x => x.Meal).ThenInclude(x => x.Tags)
            .Include(x => x.Items).ThenInclude(x => x.Meal).ThenInclude(x => x.Allergens)
            .Where(x => x.TargetCalorieRangeMin <= target && x.TargetCalorieRangeMax >= target).ToListAsync(cancellationToken);
        return OperationResultFactory.Success(new UserRecommendationDto(request.UserId, target.Value, plans.Select(x => x.ToDetails()).ToArray()));
    }
}
