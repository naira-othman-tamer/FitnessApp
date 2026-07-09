using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkoutService.Domain.Entities
{
    public class PlanDay :BaseEntity
    {
        public int WorkoutPlanId { get; set; } = 0;
        public int DayNumber { get;  set; }
        public string? Label { get;  set; } // e.g. "Push Day" — optional, display only
        public int WorkoutId { get;  set; }
        public Workout Workout { get; set; } = default!;

        public PlanDay() { }

        public static PlanDay Create(int workoutPlanId, int dayNumber, int workoutId, string? label = null)
        {
            if (dayNumber is < 1 or > 7)
                throw new ArgumentException("DayNumber must be between 1 and 7.");

            return new PlanDay
            {
                WorkoutPlanId = workoutPlanId,
                DayNumber = dayNumber,
                WorkoutId = workoutId,
                Label = label
            };
        }
    }

    public class PlanDayConfiguration : IEntityTypeConfiguration<PlanDay>
    {
        public void Configure(EntityTypeBuilder<PlanDay> builder)
        {
            builder.ToTable("PlanDays");

            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasIndex(x => new { x.WorkoutPlanId, x.DayNumber })
                   .IsUnique(); // no duplicate "Day 2" within same plan

            builder.Property(x => x.DayNumber)
                   .IsRequired();

            builder.Property(x => x.Label)
                   .HasMaxLength(100);

            builder.Property(x => x.WorkoutId)
                   .IsRequired();

            builder.HasOne(x => x.Workout)
                   .WithMany()
                   .HasForeignKey(x => x.WorkoutId)
                   .OnDelete(DeleteBehavior.Restrict);
            // don't cascade-delete plan days if a Workout is removed — Workout is reusable, must be protected
        }
    }
}
