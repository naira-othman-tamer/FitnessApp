using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutPlanDay :BaseEntity
    {
        public int WorkoutPlanId { get; set; }
        public int DayNumber { get; set; }
        public string? Name { get; set; } // e.g. "Push Day", optional label
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    }

    public class WorkoutPlanDayConfiguration : IEntityTypeConfiguration<WorkoutPlanDay>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlanDay> builder)
        {
            builder.ToTable("WorkoutPlanDays");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.WorkoutPlanId, x.DayNumber })
                .IsUnique(); // no duplicate "Day 2" within same plan

            builder.Property(x => x.WorkoutPlanId)
                   .IsRequired();

            builder.Property(x => x.DayNumber)
                   .IsRequired();

            builder.Property(x => x.Name)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.HasMany(x => x.WorkoutExercises)
                   .WithOne(e => e.WorkoutPlanDay)
                   .HasForeignKey(e => e.WorkoutPlanDayId)
                   .OnDelete(DeleteBehavior.Cascade); // deleting a day removes its exercises
        }
    }
}
