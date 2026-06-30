//using System.Text.Json;
//using FCE.Domain.Entities;
//using FCE.Domain.Enums;
//using Microsoft.EntityFrameworkCore;

//namespace FCE.Infrastructure.Data.Seed
//{
//    public static class DBSeeder
//    {
//        public static async Task SeedAsync(Context context)
//        {
//            if (await context.PlanRules.AnyAsync())
//                return;

//            var json = await File.ReadAllTextAsync(
//                Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "Seed", "PlanRules.json")
//            );

//            var records = JsonSerializer.Deserialize<List<PlanRuleJson>>(json)!;

//            var planRules = records.Select(r => TargetPlan.Create(
//                Enum.Parse<Goal>(r.Goal),
//                Enum.Parse<BMRStatus>(r.CalorieIntensityTier),
//                r.ExternalPlanId,
//                r.PlanName
//                //r.WorkoutsPerWeek
//            )).ToList();

//            await context.PlanRules.AddRangeAsync(planRules);
//            await context.SaveChangesAsync();
//        }

//        private record PlanRuleJson(
//            string Goal,
//            string CalorieIntensityTier,
//            int ExternalPlanId,
//            string PlanName
//            //int WorkoutsPerWeek
//        );
//    }
//}
