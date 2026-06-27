using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.ValueObject;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutExercise : BaseEntity
    {
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int OrderIndex { get; set; }
        public ExercisePrescription Prescription { get; set; } = default!;
    }

    public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
    {
        public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
        {
            builder.ToTable("WorkoutExercises");

            // Same-aggregate relationship: real FK + cascade delete
            builder.HasOne<Workout>()
                .WithMany(w => w.Exercise)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cross-aggregate reference: ID only, no navigation, no cascade
            builder.HasOne<Exercise>()
                .WithMany()
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(we => new { we.WorkoutId, we.OrderIndex });

            builder.OwnsOne(we => we.Prescription, p =>
            {
                p.Property(x => x.Sets).HasColumnName("SetsDefault");
                p.Property(x => x.Reps).HasColumnName("RepsDefault").HasMaxLength(20).IsRequired();
                p.Property(x => x.RestTimeInSeconds).HasColumnName("RestTimeInSeconds");
            });
        }
    }
}
