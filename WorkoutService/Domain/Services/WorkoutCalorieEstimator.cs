using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Services
{
    public static class WorkoutCalorieEstimator
    {
        private const double ReferenceWeightKg = 70.0;

        public static double Estimate(WorkoutCategory category, Difficulty difficulty, int durationMinutes)
        {
            double met = GetMetValue(category, difficulty);
            return Math.Round(met * ReferenceWeightKg * (durationMinutes / 60.0), 0);
        }

        private static double GetMetValue(WorkoutCategory category, Difficulty difficulty)
            => (category, difficulty) switch
            {
                (WorkoutCategory.FullBody, Difficulty.Beginner) => 5.0,
                (WorkoutCategory.FullBody, Difficulty.Intermediate) => 6.0,
                (WorkoutCategory.FullBody, Difficulty.Advanced) => 7.5,
                (WorkoutCategory.Chest, Difficulty.Beginner) => 3.5,
                (WorkoutCategory.Chest, Difficulty.Intermediate) => 4.5,
                (WorkoutCategory.Chest, Difficulty.Advanced) => 5.5,
                (WorkoutCategory.Arms, Difficulty.Beginner) => 3.0,
                (WorkoutCategory.Arms, Difficulty.Intermediate) => 4.0,
                (WorkoutCategory.Arms, Difficulty.Advanced) => 5.0,

                (WorkoutCategory.Shoulders, Difficulty.Beginner) => 3.0,
                (WorkoutCategory.Shoulders, Difficulty.Intermediate) => 4.0,
                (WorkoutCategory.Shoulders, Difficulty.Advanced) => 5.0,

                (WorkoutCategory.Back, Difficulty.Beginner) => 3.5,
                (WorkoutCategory.Back, Difficulty.Intermediate) => 4.5,
                (WorkoutCategory.Back, Difficulty.Advanced) => 5.5,

                (WorkoutCategory.Legs, Difficulty.Beginner) => 5.0,
                (WorkoutCategory.Legs, Difficulty.Intermediate) => 6.5,
                (WorkoutCategory.Legs, Difficulty.Advanced) => 8.0,

                (WorkoutCategory.Stomach, Difficulty.Beginner) => 3.0,
                (WorkoutCategory.Stomach, Difficulty.Intermediate) => 4.0,
                (WorkoutCategory.Stomach, Difficulty.Advanced) => 5.0,
                _ => 4.0
            };
    }
}
