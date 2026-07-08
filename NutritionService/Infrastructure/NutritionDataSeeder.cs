using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Domain.Entities;

namespace NutritionService.Infrastructure;

public interface INutritionDataSeeder { Task SeedAsync(CancellationToken cancellationToken); }

public sealed class NutritionDataSeeder(NutritionDbContext context) : INutritionDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await context.Meals.AnyAsync(cancellationToken)) return;

        var oats = Meal("11111111-1111-1111-1111-111111111111", "Protein Oats", MealType.Breakfast, 430, 30, 52, 12,
            [("Rolled oats", 60, "g"), ("Greek yogurt", 150, "g"), ("Banana", 1, "piece")], ["Mix the oats and yogurt.", "Top with sliced banana."], ["High Protein", "Vegetarian"], ["Dairy"]);
        var chicken = Meal("22222222-2222-2222-2222-222222222222", "Grilled Chicken Rice Bowl", MealType.Lunch, 620, 48, 70, 16,
            [("Chicken breast", 180, "g"), ("Cooked rice", 200, "g"), ("Mixed vegetables", 150, "g")], ["Grill the seasoned chicken.", "Serve over rice with vegetables."], ["High Protein"], []);
        var salmon = Meal("33333333-3333-3333-3333-333333333333", "Salmon and Sweet Potato", MealType.Dinner, 710, 45, 58, 28,
            [("Salmon fillet", 180, "g"), ("Sweet potato", 250, "g"), ("Broccoli", 150, "g")], ["Bake salmon and sweet potato at 200 C.", "Steam broccoli and serve."], ["Omega 3", "Gluten Free"], ["Fish"]);
        var yogurt = Meal("44444444-4444-4444-4444-444444444444", "Greek Yogurt Fruit Cup", MealType.Snack, 240, 20, 30, 4,
            [("Greek yogurt", 200, "g"), ("Mixed berries", 100, "g")], ["Combine and serve chilled."], ["High Protein", "Vegetarian"], ["Dairy"]);
        context.Meals.AddRange(oats, chicken, salmon, yogurt);

        var meals = new[] { oats, chicken, salmon, yogurt };
        context.MealPlans.AddRange(
            Plan("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1", "Lean 1700", "Calorie-controlled plan for gradual fat loss.", 1400, 1899, meals),
            Plan("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2", "Balanced 2100", "Balanced high-protein maintenance plan.", 1900, 2299, meals),
            Plan("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3", "Performance 2600", "Higher-energy plan for active and gaining users.", 2300, 3000, meals));
        await context.SaveChangesAsync(cancellationToken);
    }

    private static Meal Meal(string id, string name, MealType type, double calories, double protein, double carbs, double fat,
        (string Name, double Quantity, string Unit)[] ingredients, string[] instructions, string[] tags, string[] allergens)
    {
        var meal = new Meal { Id = Guid.Parse(id), Name = name, Type = type, Calories = calories, ProteinGrams = protein, CarbohydrateGrams = carbs, FatGrams = fat };
        foreach (var item in ingredients) meal.Ingredients.Add(new MealIngredient { Id = Guid.NewGuid(), Name = item.Name, Quantity = item.Quantity, Unit = item.Unit });
        for (var i = 0; i < instructions.Length; i++) meal.Instructions.Add(new MealInstruction { Id = Guid.NewGuid(), StepNumber = i + 1, Description = instructions[i] });
        foreach (var tag in tags) meal.Tags.Add(new MealTag { Id = Guid.NewGuid(), Name = tag });
        foreach (var allergen in allergens) meal.Allergens.Add(new MealAllergen { Id = Guid.NewGuid(), Name = allergen });
        return meal;
    }

    private static MealPlan Plan(string id, string name, string description, double minCalories, double maxCalories, Meal[] meals)
    {
        var plan = new MealPlan { Id = Guid.Parse(id), Name = name, Description = description, TargetCalorieRangeMin = minCalories, TargetCalorieRangeMax = maxCalories };
        foreach (var day in Enum.GetValues<DayOfWeek>())
            for (var index = 0; index < meals.Length; index++)
                plan.Items.Add(new MealPlanItem { Id = Guid.NewGuid(), MealId = meals[index].Id, DayOfWeek = day, MealTime = meals[index].Type, SortOrder = index + 1 });
        return plan;
    }
}
