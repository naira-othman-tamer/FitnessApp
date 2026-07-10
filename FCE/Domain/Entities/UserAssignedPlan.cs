using ContractMessages.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCE.Domain.Entities
{
    public class UserAssignedPlan  : BaseEntity
    {
        public Guid userId { get; set; }
        public Goal goal { get; private set; }
        public double calorieIntake { get; private set; } //CalroieAllocation
        public int? WorkoutPlanId { get; private set; } // Assuming this is the ID of the workout plan assigned to the user
        public string? WorkoutPlan { get; private set; } 
        public Guid? NutritionPlanId { get; private set; } // Assuming this is the ID of the nutrition plan assigned to the user
        public string? NutritionPlan { get; private set; }                                       
        public bool IsActive { get; set; } = true;

        private UserAssignedPlan() { }

        public static UserAssignedPlan Create(
            Guid userId,
            Goal goal,
            double calorieIntake,
            string? workoutPlan,
            int? workoutPlanId,
            string? nutritionPlan,
            Guid? nutritionPlanId)
            => new UserAssignedPlan
            {
                userId = userId,
                goal = goal,
                calorieIntake = calorieIntake,
                WorkoutPlan = workoutPlan,
                WorkoutPlanId = workoutPlanId,
                NutritionPlan = nutritionPlan,
                NutritionPlanId = nutritionPlanId,
                IsActive = true
            };
    }

    public class UserAssignedPlanConfiguration : IEntityTypeConfiguration<UserAssignedPlan>
    {
        public void Configure(EntityTypeBuilder<UserAssignedPlan> builder)
        {
            builder.ToTable("UserAssignedPlans");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.userId); // lookup only, no uniqueness

            builder.Property(x => x.userId)
                   .IsRequired();

            builder.Property(x => x.goal)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.calorieIntake)
                   .IsRequired();

            builder.Property(x => x.WorkoutPlan)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(x => x.NutritionPlan)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true)
                   .IsRequired();
        }
    }

}
