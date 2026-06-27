namespace WorkoutService.Domain.ValueObject
{
    public record ExercisePrescription
    {
        public int Sets { get; init; }
        public string Reps { get; init; } = default!;
        public int RestTimeInSeconds { get; init; }

        public static ExercisePrescription Create(int sets, string reps, int restTimeInSeconds)
        {
            if (sets <= 0) throw new ArgumentException("Sets must be greater than zero.");
            if (restTimeInSeconds < 0) throw new ArgumentException("Rest time cannot be negative.");
            return new ExercisePrescription { Sets = sets, Reps = reps, RestTimeInSeconds = restTimeInSeconds };
        }
    }
}
