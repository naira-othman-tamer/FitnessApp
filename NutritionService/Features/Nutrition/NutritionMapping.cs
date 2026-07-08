using NutritionService.Domain.Entities;

namespace NutritionService.Features.Nutrition;

internal static class NutritionMapping
{
    public static MealSummaryDto ToSummary(this Meal meal) => new(meal.Id, meal.Name, meal.Type, meal.Calories, meal.ProteinGrams,
        meal.CarbohydrateGrams, meal.FatGrams, meal.Tags.OrderBy(x => x.Name).Select(x => x.Name).ToArray(),
        meal.Allergens.OrderBy(x => x.Name).Select(x => x.Name).ToArray());

    public static MealDetailsDto ToDetails(this Meal meal) => new(meal.Id, meal.Name, meal.Type, meal.Calories, meal.ProteinGrams,
        meal.CarbohydrateGrams, meal.FatGrams,
        meal.Ingredients.Select(x => new IngredientDto(x.Name, x.Quantity, x.Unit)).ToArray(),
        meal.Instructions.OrderBy(x => x.StepNumber).Select(x => new InstructionDto(x.StepNumber, x.Description)).ToArray(),
        meal.Variations.Select(x => new VariationDto(x.Name, x.Description)).ToArray(),
        meal.Tags.OrderBy(x => x.Name).Select(x => x.Name).ToArray(), meal.Allergens.OrderBy(x => x.Name).Select(x => x.Name).ToArray());

    public static MealPlanDetailsDto ToDetails(this MealPlan plan) => new(plan.Id, plan.Name, plan.Description,
        plan.TargetCalorieRangeMin, plan.TargetCalorieRangeMax,
        plan.Items.OrderBy(x => x.DayOfWeek).ThenBy(x => x.SortOrder)
            .Select(x => new MealPlanItemDto(x.Id, x.DayOfWeek, x.MealTime, x.SortOrder, x.Meal.ToSummary())).ToArray());
}
