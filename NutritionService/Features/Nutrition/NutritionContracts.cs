using NutritionService.Domain.Entities;

namespace NutritionService.Features.Nutrition;

public sealed record IngredientDto(string Name, double Quantity, string Unit);
public sealed record InstructionDto(int StepNumber, string Description);
public sealed record VariationDto(string Name, string Description);
public sealed record MealSummaryDto(Guid Id, string Name, MealType Type, double Calories, double ProteinGrams, double CarbohydrateGrams, double FatGrams, string[] Tags, string[] Allergens);
public sealed record MealDetailsDto(Guid Id, string Name, MealType Type, double Calories, double ProteinGrams, double CarbohydrateGrams, double FatGrams,
    IngredientDto[] Ingredients, InstructionDto[] Instructions, VariationDto[] Variations, string[] Tags, string[] Allergens);
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount, int TotalPages);
public sealed record MealPlanSummaryDto(Guid Id, string Name, string Description, double TargetCalorieRangeMin, double TargetCalorieRangeMax);
public sealed record MealPlanItemDto(Guid Id, DayOfWeek DayOfWeek, MealType MealTime, int SortOrder, MealSummaryDto Meal);
public sealed record MealPlanDetailsDto(Guid Id, string Name, string Description, double TargetCalorieRangeMin, double TargetCalorieRangeMax, IReadOnlyList<MealPlanItemDto> Items);
public sealed record UserRecommendationDto(Guid UserId, double CalorieTarget, IReadOnlyList<MealPlanDetailsDto> MatchingPlans);
