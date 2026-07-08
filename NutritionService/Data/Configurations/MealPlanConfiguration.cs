using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Entities;

namespace NutritionService.Data.Configurations;

public sealed class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("MealPlans"); builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.TargetCalorieRangeMin).HasPrecision(10, 2);
        builder.Property(x => x.TargetCalorieRangeMax).HasPrecision(10, 2);
        builder.HasIndex(x => new { x.TargetCalorieRangeMin, x.TargetCalorieRangeMax });
    }
}

public sealed class MealPlanItemConfiguration : IEntityTypeConfiguration<MealPlanItem>
{
    public void Configure(EntityTypeBuilder<MealPlanItem> builder)
    {
        builder.ToTable("MealPlanItems"); builder.HasKey(x => x.Id);
        builder.Property(x => x.DayOfWeek).HasConversion<string>().HasMaxLength(15);
        builder.Property(x => x.MealTime).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(x => new { x.MealPlanId, x.DayOfWeek, x.MealTime, x.SortOrder }).IsUnique();
        builder.HasOne(x => x.MealPlan).WithMany(x => x.Items).HasForeignKey(x => x.MealPlanId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Meal).WithMany(x => x.MealPlanItems).HasForeignKey(x => x.MealId).OnDelete(DeleteBehavior.Restrict);
    }
}
