using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutService.Domain.Enums;

namespace WorkoutService.Domain.Entities
{
    public class WorkoutPlan : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? WorkoutDescription { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPremium { get; set; } = false;
        public Goal Goal { get; private set; }
        public int WorkoutDaysPerWeek { get; private set; }
        //public int DurationInMinutes { get; set; }
        //public double CaloriesBurn { get; set; }
        public ICollection<WorkoutPlanDay> WorkoutPlanDays { get; set; } = new List<WorkoutPlanDay>();

        private WorkoutPlan() { }

        public static WorkoutPlan Create(string name, Goal goal, int workoutDaysPerWeek,
            bool isPremium = false, string? description = null, string? imageUrl = null)
            => new WorkoutPlan
            {
                Name = name,
                Goal = goal,
                WorkoutDaysPerWeek = workoutDaysPerWeek,
                IsPremium = isPremium,
                WorkoutDescription = description,
                ImageUrl = imageUrl
            };

    }

    public class WorkoutPlanConfiguration : IEntityTypeConfiguration<WorkoutPlan>
    {
        public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
        {
            builder.ToTable("WorkoutPlans");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.Goal, x.WorkoutDaysPerWeek }); // matching lookup, not unique — multiple plans can share Goal+Days

            builder.Property(x => x.Name)
                   .HasMaxLength(150)
                   .IsRequired();

            builder.Property(x => x.WorkoutDescription)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(x => x.ImageUrl)
                   .HasMaxLength(300)
                   .IsRequired(false);

            builder.Property(x => x.IsPremium)
                   .HasDefaultValue(false)
                   .IsRequired();

            builder.Property(x => x.Goal)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.WorkoutDaysPerWeek)
                   .IsRequired();

            builder.HasMany(x => x.WorkoutPlanDays)
                   .WithOne()
                   .HasForeignKey(d => d.WorkoutPlanId)
                   .OnDelete(DeleteBehavior.Cascade); // deleting a plan removes its days
        }
    }
}

