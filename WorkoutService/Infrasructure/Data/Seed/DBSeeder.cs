using ContractMessages.Enums;
using WorkoutService.Domain.Entities;
using WorkoutService.Domain.Enums;
using WorkoutService.Domain.ValueObject;
using WorkoutService.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace Workout.Infrastructure.Data.Seed
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(Context context)
        {
            if (await context.Exercises.AnyAsync())
                return;

            var exercises = new List<Exercise>
            {
                Exercise.Create(
                    "Bodyweight Squat",
                    Difficulty.Beginner,
                    [MuscleGroup.Quads, MuscleGroup.Glutes, MuscleGroup.Hamstrings],
                    [EquipmentNeeded.BodyWeight],
                    "Foundational lower-body movement for legs and glutes."),
                Exercise.Create(
                    "Push Up",
                    Difficulty.Beginner,
                    [MuscleGroup.Chest, MuscleGroup.Shoulders, MuscleGroup.Triceps],
                    [EquipmentNeeded.BodyWeight],
                    "Upper-body pushing movement using body weight."),
                Exercise.Create(
                    "Dumbbell Row",
                    Difficulty.Beginner,
                    [MuscleGroup.Back, MuscleGroup.Biceps],
                    [EquipmentNeeded.Dumbbells, EquipmentNeeded.Bench],
                    "Horizontal pulling movement for back strength."),
                Exercise.Create(
                    "Plank",
                    Difficulty.Beginner,
                    [MuscleGroup.Core],
                    [EquipmentNeeded.BodyWeight],
                    "Isometric core stability exercise."),
                Exercise.Create(
                    "Dumbbell Romanian Deadlift",
                    Difficulty.Intermediate,
                    [MuscleGroup.Hamstrings, MuscleGroup.Glutes, MuscleGroup.Back],
                    [EquipmentNeeded.Dumbbells],
                    "Hip-hinge strength movement for posterior chain."),
                Exercise.Create(
                    "Dumbbell Shoulder Press",
                    Difficulty.Intermediate,
                    [MuscleGroup.Shoulders, MuscleGroup.Triceps],
                    [EquipmentNeeded.Dumbbells],
                    "Vertical pressing movement for shoulders."),
                Exercise.Create(
                    "Walking Lunge",
                    Difficulty.Intermediate,
                    [MuscleGroup.Quads, MuscleGroup.Glutes, MuscleGroup.Hamstrings],
                    [EquipmentNeeded.BodyWeight, EquipmentNeeded.Dumbbells],
                    "Single-leg lower-body strength and balance exercise."),
                Exercise.Create(
                    "Burpee",
                    Difficulty.Advanced,
                    [MuscleGroup.FullBody],
                    [EquipmentNeeded.BodyWeight],
                    "Full-body conditioning movement."),
                Exercise.Create(
                    "Kettlebell Swing",
                    Difficulty.Advanced,
                    [MuscleGroup.Glutes, MuscleGroup.Hamstrings, MuscleGroup.Core],
                    [EquipmentNeeded.Kettlebell],
                    "Power-focused hip-hinge conditioning movement.")
            };

            await context.Exercises.AddRangeAsync(exercises);
            await context.SaveChangesAsync();

            var exerciseIds = exercises.ToDictionary(x => x.Name, x => x.Id);

            var workouts = new List<WorkoutService.Domain.Entities.Workout>
            {
                new()
                {
                    Name = "Beginner Full Body",
                    Category = WorkoutCategory.FullBody,
                    DurationInMinutes = 35,
                    CaloriesBurn = 280,
                    IsPremium = false
                },
                new()
                {
                    Name = "Upper Body Strength",
                    Category = WorkoutCategory.UpperBody,
                    DurationInMinutes = 40,
                    CaloriesBurn = 320,
                    IsPremium = false
                },
                new()
                {
                    Name = "Lower Body Strength",
                    Category = WorkoutCategory.LowerBody,
                    DurationInMinutes = 45,
                    CaloriesBurn = 380,
                    IsPremium = false
                },
                new()
                {
                    Name = "HIIT Conditioning",
                    Category = WorkoutCategory.Hiit,
                    DurationInMinutes = 25,
                    CaloriesBurn = 420,
                    IsPremium = true
                },
                new()
                {
                    Name = "Core Stability",
                    Category = WorkoutCategory.Core,
                    DurationInMinutes = 20,
                    CaloriesBurn = 180,
                    IsPremium = false
                }
            };

            await context.Workouts.AddRangeAsync(workouts);
            await context.SaveChangesAsync();

            var workoutIds = workouts.ToDictionary(x => x.Name, x => x.Id);

            var workoutExercises = new List<WorkoutExercise>
            {
                WorkoutExercise.Create(workoutIds["Beginner Full Body"], exerciseIds["Bodyweight Squat"], 1, ExercisePrescription.Create(3, 12, 60)),
                WorkoutExercise.Create(workoutIds["Beginner Full Body"], exerciseIds["Push Up"], 2, ExercisePrescription.Create(3, 10, 60)),
                WorkoutExercise.Create(workoutIds["Beginner Full Body"], exerciseIds["Dumbbell Row"], 3, ExercisePrescription.Create(3, 12, 60)),
                WorkoutExercise.Create(workoutIds["Beginner Full Body"], exerciseIds["Plank"], 4, ExercisePrescription.Create(3, 30, 45)),

                WorkoutExercise.Create(workoutIds["Upper Body Strength"], exerciseIds["Push Up"], 1, ExercisePrescription.Create(4, 12, 60)),
                WorkoutExercise.Create(workoutIds["Upper Body Strength"], exerciseIds["Dumbbell Row"], 2, ExercisePrescription.Create(4, 10, 75)),
                WorkoutExercise.Create(workoutIds["Upper Body Strength"], exerciseIds["Dumbbell Shoulder Press"], 3, ExercisePrescription.Create(3, 10, 75)),

                WorkoutExercise.Create(workoutIds["Lower Body Strength"], exerciseIds["Bodyweight Squat"], 1, ExercisePrescription.Create(4, 15, 60)),
                WorkoutExercise.Create(workoutIds["Lower Body Strength"], exerciseIds["Dumbbell Romanian Deadlift"], 2, ExercisePrescription.Create(4, 10, 90)),
                WorkoutExercise.Create(workoutIds["Lower Body Strength"], exerciseIds["Walking Lunge"], 3, ExercisePrescription.Create(3, 12, 75)),

                WorkoutExercise.Create(workoutIds["HIIT Conditioning"], exerciseIds["Burpee"], 1, ExercisePrescription.Create(5, 10, 45)),
                WorkoutExercise.Create(workoutIds["HIIT Conditioning"], exerciseIds["Kettlebell Swing"], 2, ExercisePrescription.Create(5, 15, 45)),
                WorkoutExercise.Create(workoutIds["HIIT Conditioning"], exerciseIds["Plank"], 3, ExercisePrescription.Create(4, 30, 30)),

                WorkoutExercise.Create(workoutIds["Core Stability"], exerciseIds["Plank"], 1, ExercisePrescription.Create(4, 45, 30)),
                WorkoutExercise.Create(workoutIds["Core Stability"], exerciseIds["Burpee"], 2, ExercisePrescription.Create(3, 8, 60))
            };

            await context.WorkoutExercises.AddRangeAsync(workoutExercises);

            var plans = new List<WorkoutPlan>
            {
                new()
                {
                    Name = "Lose Weight Starter",
                    Description = "Four-day plan combining strength and conditioning for fat loss.",
                    Goal = Goal.LoseWeight,
                    WorkoutDaysPerWeek = 4,
                    IsPremium = false
                },
                new()
                {
                    Name = "Gain Weight Strength",
                    Description = "Three-day strength plan focused on progressive muscle gain.",
                    Goal = Goal.GainWeight,
                    WorkoutDaysPerWeek = 3,
                    IsPremium = false
                },
                new()
                {
                    Name = "Get Fitter Foundation",
                    Description = "Two-day beginner-friendly plan for general fitness.",
                    Goal = Goal.GetFitter,
                    WorkoutDaysPerWeek = 2,
                    IsPremium = false
                }
            };

            await context.WorkoutPlans.AddRangeAsync(plans);
            await context.SaveChangesAsync();

            var planIds = plans.ToDictionary(x => x.Name, x => x.Id);

            var planDays = new List<PlanDay>
            {
                PlanDay.Create(planIds["Lose Weight Starter"], 1, workoutIds["Beginner Full Body"], "Full Body Strength"),
                PlanDay.Create(planIds["Lose Weight Starter"], 2, workoutIds["HIIT Conditioning"], "Conditioning"),
                PlanDay.Create(planIds["Lose Weight Starter"], 3, workoutIds["Lower Body Strength"], "Lower Body"),
                PlanDay.Create(planIds["Lose Weight Starter"], 4, workoutIds["Core Stability"], "Core"),

                PlanDay.Create(planIds["Gain Weight Strength"], 1, workoutIds["Upper Body Strength"], "Upper Body"),
                PlanDay.Create(planIds["Gain Weight Strength"], 2, workoutIds["Lower Body Strength"], "Lower Body"),
                PlanDay.Create(planIds["Gain Weight Strength"], 3, workoutIds["Beginner Full Body"], "Full Body"),

                PlanDay.Create(planIds["Get Fitter Foundation"], 1, workoutIds["Beginner Full Body"], "Foundation"),
                PlanDay.Create(planIds["Get Fitter Foundation"], 2, workoutIds["Core Stability"], "Core")
            };

            await context.WorkoutPlanDays.AddRangeAsync(planDays);

            await context.SaveChangesAsync();
        }
    }
}
