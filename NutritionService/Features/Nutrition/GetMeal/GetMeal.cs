using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Data;

namespace NutritionService.Features.Nutrition.GetMeal;

public sealed record GetMealQuery(Guid Id) : IRequest<OperationResult<MealDetailsDto>>;

public sealed class GetMealHandler(NutritionDbContext context) : IRequestHandler<GetMealQuery, OperationResult<MealDetailsDto>>
{
    public async Task<OperationResult<MealDetailsDto>> Handle(GetMealQuery request, CancellationToken cancellationToken)
    {
        var meal = await context.Meals.AsNoTracking().AsSplitQuery()
            .Include(x => x.Ingredients).Include(x => x.Instructions).Include(x => x.Variations).Include(x => x.Tags).Include(x => x.Allergens)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return meal is null
            ? OperationResultFactory.NotFound<MealDetailsDto>("Meal was not found.", "لم يتم العثور على الوجبة")
            : OperationResultFactory.Success(meal.ToDetails());
    }
}
