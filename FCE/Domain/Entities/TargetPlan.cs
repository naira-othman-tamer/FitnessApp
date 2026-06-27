using FCE.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCE.Domain.Entities
{
    public class TargetPlan : BaseEntity //stage 3 [Stores derived fitness plan outputs]
    {
        public Goal goal { get; private set; }
       public CalorieTarget calorieIntake { get; private set; }
        //public double? calorieMin { get; private set; }
        //public double? calorieMax { get; private set; }
        public int ExternalPlanId { get; private set; }
        public string? PlanName { get; private set; }
       // public int WorkoutsPerWeek { get; private  set; }

        public static TargetPlan Create(
       Goal goal,
       CalorieTarget tier,
       int externalPlanId,
       string planName)
       //int workoutsPerWeek)
        {
            return new TargetPlan
            {
                goal = goal,
                calorieIntake = tier,
                ExternalPlanId = externalPlanId,
                PlanName = planName,
               // WorkoutsPerWeek = workoutsPerWeek
            };
        }
    }

    public class PlanRuleConfiguration : IEntityTypeConfiguration<TargetPlan>
    {
        public void Configure(EntityTypeBuilder<TargetPlan> builder)
        {
            builder.ToTable("PlanRules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.goal)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.calorieIntake)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.ExternalPlanId)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.PlanName)
                   .HasMaxLength(100)
                   .IsRequired();

            //builder.Property(x => x.WorkoutsPerWeek)
            //       .IsRequired();

            // One Goal + Tier combination must be unique >> enforces that seed data never has duplicate matching rules
            builder.HasIndex(x => new { x.goal, x.calorieIntake })
                   .IsUnique();
        }
    }

}
