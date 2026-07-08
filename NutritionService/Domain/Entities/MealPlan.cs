namespace NutritionService.Domain.Entities;

public sealed class MealPlan
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public double TargetCalorieRangeMin { get; set; }
    public double TargetCalorieRangeMax { get; set; }
    public ICollection<MealPlanItem> Items { get; set; } = [];
}

public sealed class MealPlanItem
{
    public Guid Id { get; set; }
    public Guid MealPlanId { get; set; }
    public Guid MealId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public MealType MealTime { get; set; }
    public int SortOrder { get; set; }
    public MealPlan MealPlan { get; set; } = null!;
    public Meal Meal { get; set; } = null!;
}
