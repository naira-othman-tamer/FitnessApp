using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.ValueObject;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutExercise : BaseEntity
    {
        public int WorkoutId { get;  set; }
        public Workout Workout { get; set; } = default!;
        public int ExerciseId { get;  set; }
        public Exercise Exercise { get;  set; } = default!;
        public int OrderIndex { get;  set; }
        public ExercisePrescription? Prescription { get;  set; } = default!;

        private WorkoutExercise() { }

        public static WorkoutExercise Create(
            int workoutId,
            int exerciseId,
            int orderIndex,
            ExercisePrescription? prescription)
        {
            if (orderIndex < 0)
                throw new ArgumentException("OrderIndex cannot be negative.");

            return new WorkoutExercise
            {
                WorkoutId = workoutId,
                ExerciseId = exerciseId,
                OrderIndex = orderIndex,
                Prescription = prescription
            };
        }
    }

    public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("WorkoutExercises");

            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasIndex(x => x.WorkoutId);

            builder.HasIndex(x => new { x.WorkoutId, x.OrderIndex })
                   .IsUnique(); // no two exercises share the same slot within a workout

            builder.Property(x => x.WorkoutId)
                   .IsRequired();

            builder.Property(x => x.ExerciseId)
                   .IsRequired();

            builder.HasOne(x => x.Exercise)
                   .WithMany()
                   .HasForeignKey(x => x.ExerciseId)
                   .OnDelete(DeleteBehavior.Restrict);
            // don't cascade-delete workout rows if an Exercise master record is removed

            builder.Property(x => x.OrderIndex)
                   .IsRequired(false);

            builder.OwnsOne(x => x.Prescription, p =>
            {
                p.Property(x => x.Sets).HasColumnName("Sets").IsRequired(false);
                p.Property(x => x.Reps).HasColumnName("Reps").IsRequired(false);
                p.Property(x => x.RestTimeInSeconds).HasColumnName("RestSeconds").IsRequired(false);
            });
        }
    }
}

