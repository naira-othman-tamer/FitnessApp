using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Entities;

namespace NutritionService.Data.Configurations;

public sealed class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("Meals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Calories).HasPrecision(10, 2);
        builder.Property(x => x.ProteinGrams).HasPrecision(10, 2);
        builder.Property(x => x.CarbohydrateGrams).HasPrecision(10, 2);
        builder.Property(x => x.FatGrams).HasPrecision(10, 2);
        builder.HasIndex(x => new { x.Type, x.Calories });
        builder.HasIndex(x => x.ProteinGrams);
    }
}

public sealed class MealIngredientConfiguration : IEntityTypeConfiguration<MealIngredient>
{
    public void Configure(EntityTypeBuilder<MealIngredient> builder)
    {
        builder.ToTable("MealIngredients"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(10, 2);
        builder.Property(x => x.Unit).HasMaxLength(30).IsRequired();
        builder.HasOne(x => x.Meal).WithMany(x => x.Ingredients).HasForeignKey(x => x.MealId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MealInstructionConfiguration : IEntityTypeConfiguration<MealInstruction>
{
    public void Configure(EntityTypeBuilder<MealInstruction> builder)
    {
        builder.ToTable("MealInstructions"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.HasIndex(x => new { x.MealId, x.StepNumber }).IsUnique();
        builder.HasOne(x => x.Meal).WithMany(x => x.Instructions).HasForeignKey(x => x.MealId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MealVariationConfiguration : IEntityTypeConfiguration<MealVariation>
{
    public void Configure(EntityTypeBuilder<MealVariation> builder)
    {
        builder.ToTable("MealVariations"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.HasOne(x => x.Meal).WithMany(x => x.Variations).HasForeignKey(x => x.MealId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MealAllergenConfiguration : IEntityTypeConfiguration<MealAllergen>
{
    public void Configure(EntityTypeBuilder<MealAllergen> builder)
    {
        builder.ToTable("MealAllergens"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();
        builder.HasIndex(x => new { x.MealId, x.Name }).IsUnique();
        builder.HasOne(x => x.Meal).WithMany(x => x.Allergens).HasForeignKey(x => x.MealId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class MealTagConfiguration : IEntityTypeConfiguration<MealTag>
{
    public void Configure(EntityTypeBuilder<MealTag> builder)
    {
        builder.ToTable("MealTags"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired();
        builder.HasIndex(x => new { x.MealId, x.Name }).IsUnique();
        builder.HasOne(x => x.Meal).WithMany(x => x.Tags).HasForeignKey(x => x.MealId).OnDelete(DeleteBehavior.Cascade);
    }
}
