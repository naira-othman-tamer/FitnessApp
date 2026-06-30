using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Domain.ValueObject;
using WorkoutService.Infrastructure.Data;

namespace Workout.Infrastructure.Data.Seed
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(Context context)
        {
            if (await context.Exercises.AnyAsync())
                return; // already seeded

            var seedFolder = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Data", "Seed");
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 1. Seed Exercises first — plans reference them by name
            var exercisesJson = await File.ReadAllTextAsync(Path.Combine(seedFolder, "Exercises.json"));
            var exerciseDtos = JsonSerializer.Deserialize<List<ExerciseSeedDto>>(exercisesJson, jsonOptions)!;

            var exercises = exerciseDtos.Select(e => new Exercise
            {
                Name = e.Name,
                Description = e.Description,
                ImageURL = e.ImageURL,
                TargetMuscles = e.TargetMuscles,
                EquipmentNeeded = e.EquipmentNeeded.Select(Enum.Parse<EquipmentNeeded>).ToList(),
                Difficulty = Enum.Parse<Difficulty>(e.Difficulty)
            }).ToList();

            await context.Exercises.AddRangeAsync(exercises);
            await context.SaveChangesAsync(); // need generated Ids before linking WorkoutExercises

            var exerciseByName = exercises.ToDictionary(e => e.Name, e => e);

            // 2. Seed Workout Plans (+ Days + Exercises), linking by exercise name
            var plansJson = await File.ReadAllTextAsync(Path.Combine(seedFolder, "WorkoutPlans.json"));
            var planDtos = JsonSerializer.Deserialize<List<WorkoutPlanSeedDto>>(plansJson, jsonOptions)!;

            foreach (var planDto in planDtos)
            {
                var plan = WorkoutPlan.Create(
                    planDto.Name,
                    Enum.Parse<Goal>(planDto.Goal),
                    planDto.WorkoutDaysPerWeek,
                    planDto.IsPremium,
                    planDto.Description);

                foreach (var dayDto in planDto.Days)
                {
                    var day = new WorkoutPlanDay
                    {
                        DayNumber = dayDto.DayNumber,
                        Name = dayDto.Name
                    };

                    foreach (var exDto in dayDto.Exercises)
                    {
                        if (!exerciseByName.TryGetValue(exDto.ExerciseName, out var exercise))
                            throw new InvalidOperationException(
                                $"Seed error: exercise '{exDto.ExerciseName}' referenced in WorkoutPlans.json was not found in exercises.json.");

                        day.WorkoutExercises.Add(new WorkoutExercise
                        {
                            Exercise = exercise,
                            OrderIndex = exDto.OrderIndex,
                            Prescription = ExercisePrescription.Create(exDto.Sets, exDto.Reps, exDto.RestSeconds)
                        });
                    }

                    plan.WorkoutPlanDays.Add(day);
                }

                await context.WorkoutPlans.AddAsync(plan);
            }

            await context.SaveChangesAsync();
        }

        private record ExerciseSeedDto(
            string Name,
            string? Description,
            string? ImageURL,
            List<string> TargetMuscles,
            List<string> EquipmentNeeded,
            string Difficulty);

        private record WorkoutPlanSeedDto(
            string Name,
            string Goal,
            int WorkoutDaysPerWeek,
            bool IsPremium,
            string? Description,
            List<WorkoutPlanDaySeedDto> Days);

        private record WorkoutPlanDaySeedDto(
            int DayNumber,
            string? Name,
            List<WorkoutExerciseSeedDto> Exercises);

        private record WorkoutExerciseSeedDto(
            string ExerciseName,
            int OrderIndex,
            int Sets,
            int Reps,
            int RestSeconds);
    }
}