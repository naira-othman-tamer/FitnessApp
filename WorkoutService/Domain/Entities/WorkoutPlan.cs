using ContractMessages.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutPlan : BaseEntity
    {
        public string Name { get; private set; } = default!;
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public bool IsPremium { get; private set; }
        public Goal Goal { get; private set; }
        public int WorkoutDaysPerWeek { get; private set; }
        public Difficulty Difficulty { get; private set; }

        public ICollection<PlanDay> PlanDays { get; private set; } = new List<PlanDay>();

        private WorkoutPlan() { }

        public static WorkoutPlan Create(
            string name,
            Goal goal,
            int workoutDaysPerWeek,
            Difficulty difficulty,
            bool isPremium = false,
            string? description = null,
            string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.");
            if (workoutDaysPerWeek is < 1 or > 7)
                throw new ArgumentException("WorkoutDaysPerWeek must be between 1 and 7.");

            return new WorkoutPlan
            {
                Name = name,
                Goal = goal,
                WorkoutDaysPerWeek = workoutDaysPerWeek,
                Difficulty = difficulty,
                IsPremium = isPremium,
                Description = description,
                ImageUrl = imageUrl
            };
        }
    }

    public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
        {
            builder.ToTable("WorkoutPlans");

            builder.HasKey(x => x.Id);

            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasIndex(x => new { x.Goal, x.WorkoutDaysPerWeek });
            // matching lookup for FCE requests — not unique, multiple plans can share Goal+Days

            builder.Property(x => x.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(300);

            builder.Property(x => x.IsPremium)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(x => x.Goal)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.Difficulty)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.WorkoutDaysPerWeek)
                   .IsRequired();

            builder.HasMany(x => x.PlanDays)
                   .WithOne()
                   .HasForeignKey(d => d.WorkoutPlanId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

