namespace NutritionService.Domain.Entities;

public sealed class MealIngredient
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
    public required string Name { get; set; }
    public double Quantity { get; set; }
    public required string Unit { get; set; }
    public Meal Meal { get; set; } = null!;
}

public sealed class MealInstruction
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
    public int StepNumber { get; set; }
    public required string Description { get; set; }
    public Meal Meal { get; set; } = null!;
}

public sealed class MealVariation
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public Meal Meal { get; set; } = null!;
}

public sealed class MealAllergen
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
    public required string Name { get; set; }
    public Meal Meal { get; set; } = null!;
}

public sealed class MealTag
{
    public Guid Id { get; set; }
    public Guid MealId { get; set; }
    public required string Name { get; set; }
    public Meal Meal { get; set; } = null!;
}
