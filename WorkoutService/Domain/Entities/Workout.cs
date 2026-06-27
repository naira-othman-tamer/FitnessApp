using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;
using WorkoutService.Domain.Services;

namespace WorkoutService.Domain.Entities
{
    public class Workout : BaseEntity
    {
        public int WorkoutPlanId { get; set; }
        public WorkoutCategory Category { get; set; }
        public string Name { get; set; } = default!;
        public string? WorkoutDescription { get; set; }
        public Difficulty WorkoutDifficulty { get; set; }
        public int DurationInMinutes { get; set; }
        public double CaloriesBurn { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPremium { get; set; }=false;
        public ICollection<WorkoutExercise> Exercise { get; set; } = new List<WorkoutExercise>();

        private Workout() { } 

        public static Workout Create(
            int workoutPlanId, string name, WorkoutCategory category,
            Difficulty difficulty, int durationMinutes, bool isPremium,
            double? manualCaloriesBurn = null)
        {
            return new Workout
            {
                WorkoutPlanId = workoutPlanId,
                Name = name,
                Category = category,
                WorkoutDifficulty = difficulty,
                DurationInMinutes = durationMinutes,
                IsPremium = isPremium,
                CaloriesBurn = manualCaloriesBurn ?? WorkoutCalorieEstimator.Estimate(category, difficulty, durationMinutes)
            };
        }

    }

    public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
    {
        public void Configure(EntityTypeBuilder<Workout> builder)
        {
            builder.ToTable("Workouts");

            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(w => w.WorkoutDescription)
                .HasMaxLength(500);

            builder.Property(w => w.ImageUrl)
                .HasMaxLength(500);

            builder.Property(w => w.Category)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(w => w.WorkoutDifficulty)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(w => w.Category);
            builder.HasIndex(w => w.WorkoutPlanId);

            builder.HasOne<WorkoutPlan>()
                .WithMany(p => p.Workouts)
                .HasForeignKey(w => w.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.Exercise)
                .WithOne()
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
