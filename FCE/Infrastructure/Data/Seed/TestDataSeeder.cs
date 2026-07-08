using ContractMessages.Enums;
using FCE.Domain.Aggregates;
using FCE.Domain.Entities;
using FCE.Domain.Enums;
using FCE.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;

namespace FCE.Infrastructure.Data.Seed
{
    public static class TestDataSeeder
    {
        public static async Task SeedAsync(Context context)
        {
            var users = new[]
            {
                new SeedUser(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    new PhysicalStats(80, 175, 28, Gender.Male),
                    Goal.LoseWeight,
                    ActivityLevel.Intermediate,
                    4,
                    101,
                    "Lean Strength Plan",
                    "Lean 1700"),
                new SeedUser(
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    new PhysicalStats(58, 162, 24, Gender.Female),
                    Goal.GainWeight,
                    ActivityLevel.Beginner,
                    3,
                    102,
                    "Beginner Muscle Plan",
                    "Balanced 2100"),
                new SeedUser(
                    Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    new PhysicalStats(95, 180, 35, Gender.Male),
                    Goal.GetFitter,
                    ActivityLevel.Rookie,
                    2,
                    103,
                    "Starter Fitness Plan",
                    "Performance 2600")
            };

            foreach (var user in users)
            {
                var hasFitnessStats = await context.UserFitnessStats
                    .AnyAsync(x => x.userId == user.UserId);

                if (!hasFitnessStats)
                {
                    await context.UserFitnessStats.AddAsync(new UserFitnessStats
                    {
                        userId = user.UserId,
                        PhysicalStats = user.Stats,
                        goal = user.Goal,
                        activityLevel = user.Activity,
                        WorkoutDays = user.WorkoutDays,
                        IsActive = true
                    });
                }

                var hasCalculatedMetrics = await context.CalculatedMetrics
                    .AnyAsync(x => x.UserId == user.UserId);

                var metrics = CalculatedMetrics.Calculate(
                    user.UserId,
                    user.Stats,
                    user.Activity,
                    user.Goal,
                    user.Stats.Gender);

                if (!hasCalculatedMetrics)
                    await context.CalculatedMetrics.AddAsync(metrics);

                var hasActivePlan = await context.UserAssignedPlans
                    .AnyAsync(x => x.userId == user.UserId && x.IsActive);

                if (!hasActivePlan)
                {
                    await context.UserAssignedPlans.AddAsync(UserAssignedPlan.Create(
                        user.UserId,
                        user.Goal,
                        metrics.CalorieTarget,
                        user.WorkoutPlanName,
                        user.WorkoutPlanId,
                        user.NutritionPlanName,
                        null));
                }

                var hasPlanHistory = await context.UserPlanHistories
                    .AnyAsync(x => x.UserId == user.UserId && x.PlanId == user.WorkoutPlanId);

                if (!hasPlanHistory)
                {
                    await context.UserPlanHistories.AddAsync(new UserPlanHistory
                    {
                        UserId = user.UserId,
                        PlanId = user.WorkoutPlanId,
                        ResonForChange = "Initial seed assignment"
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        private sealed record SeedUser(
            Guid UserId,
            PhysicalStats Stats,
            Goal Goal,
            ActivityLevel Activity,
            int WorkoutDays,
            int WorkoutPlanId,
            string WorkoutPlanName,
            string NutritionPlanName);
    }
}
