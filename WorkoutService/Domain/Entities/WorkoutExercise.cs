using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.ValueObject;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutExercise : BaseEntity
    {
        public int WorkoutPlanDayId { get; set; }
        public WorkoutPlanDay WorkoutPlanDay { get; set; } = default!;
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = default!;
        public int OrderIndex { get; set; }
        public ExercisePrescription Prescription { get; set; } = default!;
    }

    public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("WorkoutExercises");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.WorkoutPlanDayId, x.OrderIndex }); // ordered reads per day

            builder.Property(x => x.WorkoutPlanDayId)
                   .IsRequired();

            builder.Property(x => x.ExerciseId)
                   .IsRequired();

            builder.HasOne(x => x.Exercise)
                   .WithMany()
                   .HasForeignKey(x => x.ExerciseId)
                   .OnDelete(DeleteBehavior.Restrict); // don't cascade-delete plan rows if an exercise is removed

            builder.Property(x => x.OrderIndex)
                   .IsRequired();

            builder.OwnsOne(x => x.Prescription, p =>
            {
                p.Property(x => x.Sets).HasColumnName("Sets").IsRequired();
                p.Property(x => x.Reps).HasColumnName("Reps").IsRequired();
                p.Property(x => x.RestTimeInSeconds).HasColumnName("RestSeconds").IsRequired();
            });
        }
    }
}

