using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Domain.Entities;
using Repository.Layer.Interfaces;

namespace NutritionService.Features.Nutrition.GetRecommendations;

public sealed record GetRecommendationsQuery(string? MealType, int Page, int PageSize, double? MaxCalories, double? MinProtein)
    : IRequest<OperationResult<PagedResponse<MealSummaryDto>>>;

public sealed class GetRecommendationsHandler(IUnitOfWork<NutritionDbContext> unitOfWork)
    : IRequestHandler<GetRecommendationsQuery, OperationResult<PagedResponse<MealSummaryDto>>>
{
    public async Task<OperationResult<PagedResponse<MealSummaryDto>>> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        if (request.Page < 1 || request.PageSize is < 1 or > 100 || request.MaxCalories < 0 || request.MinProtein < 0)
            return OperationResultFactory.BadRequest<PagedResponse<MealSummaryDto>>("Invalid pagination or nutrition filter values.", "قيم التصفية أو ترقيم الصفحات غير صالحة");

        var query = unitOfWork.Repository<Meal, Guid>().Query(true, x => x.Tags, x => x.Allergens);
        if (!string.IsNullOrWhiteSpace(request.MealType))
        {
            if (!Enum.TryParse<MealType>(request.MealType, true, out var type))
                return OperationResultFactory.BadRequest<PagedResponse<MealSummaryDto>>("Meal type must be Breakfast, Lunch, Dinner, or Snack.", "نوع الوجبة غير صالح");
            query = query.Where(x => x.Type == type);
        }
        if (request.MaxCalories is not null) query = query.Where(x => x.Calories <= request.MaxCalories);
        if (request.MinProtein is not null) query = query.Where(x => x.ProteinGrams >= request.MinProtein);

        var total = await query.CountAsync(cancellationToken);
        var meals = await query.OrderBy(x => x.Calories).ThenBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
        var response = new PagedResponse<MealSummaryDto>(meals.Select(x => x.ToSummary()).ToArray(), request.Page, request.PageSize,
            total, (int)Math.Ceiling(total / (double)request.PageSize));
        return OperationResultFactory.Success(response);
    }
}
