using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutPlan : BaseEntity
    {
        public int ExternalPlanId { get; set; }
        //public string? WorkoutPlanName { get; set; }
        public string? PlanDescription { get; set; }
        public Difficulty? Difficulty { get; set; }
        public ICollection<Workout> Workouts { get; set; } = new List<Workout>();

    }

    public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
        {
            builder.ToTable("WorkoutPlans");

            builder.HasIndex(p => p.ExternalPlanId).IsUnique();

            builder.Property(p => p.PlanDescription)
                .HasMaxLength(500);

            builder.Property(p => p.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }


}
