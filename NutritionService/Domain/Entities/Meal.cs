namespace NutritionService.Domain.Entities;

public enum MealType { Breakfast, Lunch, Dinner, Snack }

public sealed class Meal
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public MealType Type { get; set; }
    public double Calories { get; set; }
    public double ProteinGrams { get; set; }
    public double CarbohydrateGrams { get; set; }
    public double FatGrams { get; set; }
    public ICollection<MealIngredient> Ingredients { get; set; } = [];
    public ICollection<MealInstruction> Instructions { get; set; } = [];
    public ICollection<MealVariation> Variations { get; set; } = [];
    public ICollection<MealAllergen> Allergens { get; set; } = [];
    public ICollection<MealTag> Tags { get; set; } = [];
    public ICollection<MealPlanItem> MealPlanItems { get; set; } = [];
}
