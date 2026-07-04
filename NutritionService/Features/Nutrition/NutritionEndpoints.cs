using MediatR;
using NutritionService.Features.Nutrition.GetMeal;
using NutritionService.Features.Nutrition.GetMealPlans;
using NutritionService.Features.Nutrition.GetRecommendations;
using NutritionService.Features.Nutrition.GetUserRecommendations;

namespace NutritionService.Features.Nutrition;

public static class NutritionEndpoints
{
    public static IEndpointRouteBuilder MapNutritionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/nutrition").RequireAuthorization().WithTags("Nutrition");
        group.MapGet("/recommendations", async (string? mealType, int? page, int? pageSize, double? maxCalories, double? minProtein, IMediator mediator, CancellationToken ct) =>
            (await mediator.Send(new GetRecommendationsQuery(mealType, page ?? 1, pageSize ?? 20, maxCalories, minProtein), ct)).ToHttpResult());
        group.MapGet("/recommendations/{userId:guid}", async (Guid userId, IMediator mediator, CancellationToken ct) =>
            (await mediator.Send(new GetUserRecommendationsQuery(userId), ct)).ToHttpResult());
        group.MapGet("/meals/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
            (await mediator.Send(new GetMealQuery(id), ct)).ToHttpResult());
        group.MapGet("/meal-plans", async (IMediator mediator, CancellationToken ct) =>
            (await mediator.Send(new GetMealPlansQuery(), ct)).ToHttpResult());
        group.MapGet("/meal-plans/by-calories", async (double calories, IMediator mediator, CancellationToken ct) =>
            (await mediator.Send(new GetMealPlansByCaloriesQuery(calories), ct)).ToHttpResult());
        return endpoints;
    }
}
