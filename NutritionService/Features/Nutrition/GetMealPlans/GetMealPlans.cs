using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Domain.Entities;
using Repository.Layer.Interfaces;

namespace NutritionService.Features.Nutrition.GetMealPlans;

public sealed record GetMealPlansQuery : IRequest<OperationResult<IReadOnlyList<MealPlanSummaryDto>>>;
public sealed record GetMealPlansByCaloriesQuery(double Calories) : IRequest<OperationResult<IReadOnlyList<MealPlanDetailsDto>>>;

public sealed class GetMealPlansHandler(IUnitOfWork<NutritionDbContext> unitOfWork) :
    IRequestHandler<GetMealPlansQuery, OperationResult<IReadOnlyList<MealPlanSummaryDto>>>,
    IRequestHandler<GetMealPlansByCaloriesQuery, OperationResult<IReadOnlyList<MealPlanDetailsDto>>>
{
    public async Task<OperationResult<IReadOnlyList<MealPlanSummaryDto>>> Handle(GetMealPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await unitOfWork.Repository<MealPlan, Guid>().Query().OrderBy(x => x.TargetCalorieRangeMin)
            .Select(x => new MealPlanSummaryDto(x.Id, x.Name, x.Description, x.TargetCalorieRangeMin, x.TargetCalorieRangeMax)).ToListAsync(cancellationToken);
        return OperationResultFactory.Success<IReadOnlyList<MealPlanSummaryDto>>(plans);
    }

    public async Task<OperationResult<IReadOnlyList<MealPlanDetailsDto>>> Handle(GetMealPlansByCaloriesQuery request, CancellationToken cancellationToken)
    {
        if (request.Calories <= 0) return OperationResultFactory.BadRequest<IReadOnlyList<MealPlanDetailsDto>>("Calories must be greater than zero.", "يجب أن تكون السعرات أكبر من صفر");
        var plans = await QueryPlans(request.Calories, cancellationToken);
        return OperationResultFactory.Success<IReadOnlyList<MealPlanDetailsDto>>(plans.Select(x => x.ToDetails()).ToArray());
    }

    internal async Task<List<MealPlan>> QueryPlans(double calories, CancellationToken cancellationToken) =>
        await unitOfWork.Repository<MealPlan, Guid>().Query().AsSplitQuery().Include(x => x.Items).ThenInclude(x => x.Meal).ThenInclude(x => x.Tags)
            .Include(x => x.Items).ThenInclude(x => x.Meal).ThenInclude(x => x.Allergens)
            .Where(x => x.TargetCalorieRangeMin <= calories && x.TargetCalorieRangeMax >= calories)
            .OrderBy(x => x.TargetCalorieRangeMin).ToListAsync(cancellationToken);
}
