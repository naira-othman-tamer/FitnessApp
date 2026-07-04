using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Domain.Entities;
using Repository.Layer.Interfaces;

namespace NutritionService.Features.Nutrition.GetMeal;

public sealed record GetMealQuery(Guid Id) : IRequest<OperationResult<MealDetailsDto>>;

public sealed class GetMealHandler(IUnitOfWork<NutritionDbContext> unitOfWork) : IRequestHandler<GetMealQuery, OperationResult<MealDetailsDto>>
{
    public async Task<OperationResult<MealDetailsDto>> Handle(GetMealQuery request, CancellationToken cancellationToken)
    {
        var meal = await unitOfWork.Repository<Meal, Guid>()
            .Query(true, x => x.Ingredients, x => x.Instructions, x => x.Variations, x => x.Tags, x => x.Allergens)
            .AsSplitQuery()
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        return meal is null
            ? OperationResultFactory.NotFound<MealDetailsDto>("Meal was not found.", "لم يتم العثور على الوجبة")
            : OperationResultFactory.Success(meal.ToDetails());
    }
}
