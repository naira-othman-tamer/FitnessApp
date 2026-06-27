using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Domain.ValueObject;
using WorkoutService.Infrastructure.Data;

namespace WorkoutService.Infrasructure.Data.DataSeeder
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(Context context)
        {
            if (!await context.Exercises.AnyAsync())
                await SeedExercisesAsync(context);

            if (!await context.WorkoutPlans.AnyAsync())
                await SeedWorkoutPlansAsync(context);
        }

        private static async Task SeedExercisesAsync(Context context)
        {
            var json = await File.ReadAllTextAsync(
                Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "Seed", "ExercisesSeed.json"));

            var records = JsonSerializer.Deserialize<List<ExerciseJson>>(json)!;

            var exercises = records.Select(r => new Exercise
            {
                Name = r.Name,
                Description = r.Description,
                VideoUrl = r.VideoUrl,
                TargetMuscles = r.TargetMuscles,
                EquipmentNeeded = r.EquipmentNeeded,
                ExerciseDifficulty = Enum.Parse<Difficulty>(r.Difficulty)
            }).ToList();

            await context.Exercises.AddRangeAsync(exercises);
            await context.SaveChangesAsync();
        }

        private static async Task SeedWorkoutPlansAsync(Context context)
        {
            var json = await File.ReadAllTextAsync(
                Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "Seed", "WorkoutPlansSeed.json"));

            var plansJson = JsonSerializer.Deserialize<List<WorkoutPlanJson>>(json)!;
            var exerciseIds = await context.Exercises.ToDictionaryAsync(e => e.Name, e => e.Id);

            foreach (var planJson in plansJson)
            {
                var plan = new WorkoutPlan
                {
                    ExternalPlanId = planJson.ExternalPlanId,
                    PlanDescription = planJson.PlanDescription,
                    Difficulty = Enum.Parse<Difficulty>(planJson.Difficulty)
                };
                await context.WorkoutPlans.AddAsync(plan);
                await context.SaveChangesAsync(); // need plan.Id

                foreach (var workoutJson in planJson.Workouts)
                {
                    var workout = Workout.Create(
                        plan.Id, workoutJson.Name, Enum.Parse<WorkoutCategory>(workoutJson.Category), 
                        Enum.Parse<Difficulty>(workoutJson.WorkoutDifficulty),
                        workoutJson.DurationInMinutes, workoutJson.IsPremium);

                    await context.Workouts.AddAsync(workout);
                    await context.SaveChangesAsync(); // need workout.Id

                    foreach (var exJson in workoutJson.Exercises)
                    {
                        var workoutExercise = new WorkoutExercise
                        {
                            WorkoutId = workout.Id,
                            ExerciseId = exerciseIds[exJson.ExerciseName],
                            OrderIndex = exJson.OrderIndex,
                            Prescription = ExercisePrescription.Create(exJson.Sets, exJson.Reps, exJson.RestSeconds)
                        };
                        await context.WorkoutExercises.AddAsync(workoutExercise);
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        private record ExerciseJson(string Name, string? Description, string? VideoUrl,
            List<string> TargetMuscles, List<string> EquipmentNeeded, string Difficulty);

        private record WorkoutPlanJson(int ExternalPlanId, string? PlanDescription, string Difficulty,
            List<WorkoutJson> Workouts);

        private record WorkoutJson(string Name, string? WorkoutDescription, string Category,
            string WorkoutDifficulty, int DurationInMinutes, string? ImageUrl, bool IsPremium,
            List<WorkoutExerciseJson> Exercises);

        private record WorkoutExerciseJson(string ExerciseName, int OrderIndex, int Sets, string Reps, int RestSeconds);
    }
}
