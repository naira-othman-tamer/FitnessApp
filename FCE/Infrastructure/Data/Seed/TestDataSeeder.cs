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
            //if (await context.UserFitnessStats.AnyAsync())
            //    return;

            //var users = new[]
            //{
            //    new
            //    {
            //        UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            //        Stats = new PhysicalStats(80, 175, 28, Gender.Male),
            //        Goal = Goal.LoseWeight,
            //        Activity = ActivityLevel.Intermediate,
            //        WorkoutDays = 4
            //    },
            //    new
            //    {
            //        UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            //        Stats = new PhysicalStats(58, 162, 24, Gender.Female),
            //        Goal = Goal.GainWeight,
            //        Activity = ActivityLevel.Beginner,
            //        WorkoutDays = 3
            //    },
            //    new
            //    {
            //        UserId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            //        Stats = new PhysicalStats(95, 180, 35, Gender.Male),
            //        Goal = Goal.GetFitter,
            //        Activity = ActivityLevel.Rookie,
            //        WorkoutDays = 2
            //    }
            //};

            //foreach (var u in users)
            //{
            //    var fitnessStats = new UserFitnessStats
            //    {
            //        userId = u.UserId,
            //        PhysicalStats = u.Stats,
            //        goal = u.Goal,
            //        activityLevel = u.Activity,
            //        WorkoutDays = u.WorkoutDays,
            //        IsActive = true
            //    };

            //    var metrics = CalculatedMetrics.Calculate(
            //        u.UserId, u.Stats, u.Activity, u.Goal, u.Stats.Gender);

            //    var assignedPlan = UserAssignedPlan.Create(
            //        u.UserId,
            //        u.Goal,
            //        metrics.CalorieTarget,
            //        $"workout-plan-{u.Goal}".ToLower(),
            //        $"nutrition-plan-{u.Goal}".ToLower());

            //    await context.UserFitnessStats.AddAsync(fitnessStats);
            //    await context.CalculatedMetrics.AddAsync(metrics);
            //    await context.UserAssignedPlans.AddAsync(assignedPlan);
            //}

            //await context.SaveChangesAsync();
        }
    }
}